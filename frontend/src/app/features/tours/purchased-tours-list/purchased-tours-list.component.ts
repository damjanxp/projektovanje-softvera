import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { PurchaseService } from '../../../core/services/purchase.service';
import { TourService } from '../../../core/services/tour.service';
import { TourDto, InterestLabels, DifficultyLabels, Interest, Difficulty } from '../../../shared/models';

interface PurchasedTourInfo {
  tour: TourDto;
  purchasedAt: string;
  priceAtPurchase: number;
}

@Component({
  selector: 'app-purchased-tours-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './purchased-tours-list.component.html',
  styleUrl: './purchased-tours-list.component.css'
})
export class PurchasedToursListComponent implements OnInit {
  private purchaseService = inject(PurchaseService);
  private tourService = inject(TourService);

  purchasedTours: PurchasedTourInfo[] = [];
  loading = false;
  error: string | null = null;

  ngOnInit(): void {
    this.loadPurchasedTours();
  }

  loadPurchasedTours(): void {
    this.loading = true;
    this.error = null;

    this.purchaseService.getMyPurchases().subscribe({
      next: (response) => {
        if (response.success && response.data) {
          // Flatten all tours from all purchases
          const allTourIds: string[] = [];
          const tourPurchaseMap = new Map<string, { purchasedAt: string, price: number }>();
          
          response.data.forEach(purchase => {
            purchase.tours.forEach(tour => {
              if (!tourPurchaseMap.has(tour.tourId)) {
                allTourIds.push(tour.tourId);
                tourPurchaseMap.set(tour.tourId, {
                  purchasedAt: purchase.purchasedAt,
                  price: tour.priceAtPurchase
                });
              }
            });
          });

          if (allTourIds.length === 0) {
            this.loading = false;
            this.purchasedTours = [];
            return;
          }

          // Fetch full tour details for each tour
          const tourObservables = allTourIds.map(tourId =>
            this.tourService.getTourById(tourId).pipe(
              map(tourResponse => ({ tourId, tourResponse })),
              catchError(() => of({ tourId, tourResponse: null }))
            )
          );

          forkJoin(tourObservables).subscribe({
            next: (results) => {
              this.loading = false;
              this.purchasedTours = results
                .filter(result => result.tourResponse?.success && result.tourResponse?.data)
                .map(result => {
                  const tourData = result.tourResponse!.data!;
                  const purchaseInfo = tourPurchaseMap.get(result.tourId)!;
                  return {
                    tour: tourData,
                    purchasedAt: purchaseInfo.purchasedAt,
                    priceAtPurchase: purchaseInfo.price
                  };
                });
            },
            error: () => {
              this.loading = false;
              this.error = 'An error occurred while loading tour details';
            }
          });
        } else {
          this.loading = false;
          this.error = response.error?.message || 'Failed to load purchased tours';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading your purchases';
      }
    });
  }

  getCategoryLabel(category: number): string {
    return InterestLabels[category as Interest] || 'Unknown';
  }

  getDifficultyLabel(difficulty: number): string {
    return DifficultyLabels[difficulty as Difficulty] || 'Unknown';
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatPrice(price: number): string {
    return `$${price.toFixed(2)}`;
  }
}
