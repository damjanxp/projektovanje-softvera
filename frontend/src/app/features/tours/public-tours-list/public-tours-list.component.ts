import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TourService, CartService, AuthService, RatingService, PurchaseService } from '../../../core/services';
import { ApiResponse, TourDto, InterestLabels, DifficultyLabels, Interest, Difficulty, TourStatus, PurchaseResultDto } from '../../../shared/models';
import { StatusBadgeComponent } from '../../../shared/components';

interface TourWithRating extends TourDto {
  averageRating?: number | null;
}

@Component({
  selector: 'app-public-tours-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, StatusBadgeComponent],
  templateUrl: './public-tours-list.component.html',
  styleUrl: './public-tours-list.component.css'
})
export class PublicToursListComponent implements OnInit {
private tourService = inject(TourService);
private cartService = inject(CartService);
private authService = inject(AuthService);
private ratingService = inject(RatingService);
private purchaseService = inject(PurchaseService);
  
tours: TourWithRating[] = [];
purchasedTourIds = new Set<string>();
loading = false;
error: string | null = null;
sortOrder: 'asc' | 'desc' = 'asc';
addingToCartId: string | null = null;

  ngOnInit(): void {
    this.loadTours();
    if (this.isTourist) {
      this.loadCartItems();
      this.loadPurchasedTours();
    }
  }

  loadTours(): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getPublishedTours(this.sortOrder).subscribe({
      next: (response: ApiResponse<TourDto[]>) => {
        if (response.success && response.data) {
          const tours = response.data;
          
          // Fetch ratings for all tours
          const ratingObservables = tours.map(tour =>
            this.ratingService.getAverageRatingForTour(tour.id).pipe(
              map(ratingResponse => ({
                ...tour,
                averageRating: ratingResponse.success ? (ratingResponse.data ?? null) : null
              })),
              catchError(() => of({ ...tour, averageRating: null }))
            )
          );

          forkJoin(ratingObservables).subscribe({
            next: (toursWithRatings) => {
              this.loading = false;
              this.tours = toursWithRatings;
            },
            error: () => {
              this.loading = false;
              this.tours = tours.map(tour => ({ ...tour, averageRating: null }));
            }
          });
        } else {
          this.loading = false;
          this.error = response.error?.message || 'Failed to load tours';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading tours';
      }
    });
  }

  loadCartItems(): void {
    this.cartService.getCart().subscribe();
  }

  loadPurchasedTours(): void {
    this.purchaseService.getMyPurchases().subscribe({
      next: (response: ApiResponse<PurchaseResultDto[]>) => {
        if (response.success && response.data) {
          this.purchasedTourIds = new Set(
            response.data.flatMap(p => p.tours.map(t => t.tourId))
          );
        }
      },
      error: () => {}
    });
  }

  addToCart(tourId: string): void {
    this.addingToCartId = tourId;
    
    this.cartService.addToCart(tourId).subscribe({
      next: (response: ApiResponse<void>) => {
        this.addingToCartId = null;
        if (!response.success) {
          this.error = response.error?.message || 'Failed to add to cart';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.addingToCartId = null;
        this.error = err.error?.error?.message || 'An error occurred while adding to cart';
      }
    });
  }

  isInCart(tourId: string): boolean {
    return this.cartService.isInCartSync(tourId);
  }

  get isTourist(): boolean {
    return this.authService.getUserRole() === 'Tourist';
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  onSortChange(): void {
    this.loadTours();
  }

  getCategoryLabel(category: Interest): string {
    return InterestLabels[category] || 'Unknown';
  }

  getDifficultyLabel(difficulty: Difficulty): string {
    return DifficultyLabels[difficulty] || 'Unknown';
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatPrice(price: number): string {
      return `$${price.toFixed(2)}`;
    }

    getTourImage(tour: TourDto): string {
      const firstKp = tour.keyPoints?.length
        ? [...tour.keyPoints].sort((a, b) => a.order - b.order)[0]
        : null;
      if (firstKp?.imageUrl) {
        return `url('${firstKp.imageUrl}')`;
      }
      return '';
    }

    isCanceled(tour: TourDto): boolean {
        return tour.status === TourStatus.Canceled;
      }

      hasReplacementRequested(tour: TourDto): boolean {
        return tour.needsReplacement === true;
      }

      canAddToCart(tour: TourDto): boolean {
        return this.isTourist 
          && !this.isCanceled(tour) 
          && !this.hasReplacementRequested(tour) 
          && !this.isInCart(tour.id) 
          && !this.isAlreadyPurchased(tour.id) 
          && !this.isTourPast(tour);
      }

      isAlreadyPurchased(tourId: string): boolean {
        return this.purchasedTourIds.has(tourId);
      }

      isTourPast(tour: TourDto): boolean {
        return new Date(tour.startDate) < new Date();
      }
    }
