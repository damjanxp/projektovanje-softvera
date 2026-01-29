import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TourService, CartService, AuthService } from '../../../core/services';
import { ApiResponse, TourDto, InterestLabels, DifficultyLabels, Interest, Difficulty, TourStatus } from '../../../shared/models';
import { StatusBadgeComponent } from '../../../shared/components';

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
  
  tours: TourDto[] = [];
  loading = false;
  error: string | null = null;
  sortOrder: 'asc' | 'desc' = 'asc';
  addingToCartId: string | null = null;

  ngOnInit(): void {
    this.loadTours();
    if (this.isTourist) {
      this.loadCartItems();
    }
  }

  loadTours(): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getPublishedTours(this.sortOrder).subscribe({
      next: (response: ApiResponse<TourDto[]>) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tours = response.data;
        } else {
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

    isCanceled(tour: TourDto): boolean {
      return tour.status === TourStatus.Canceled;
    }

    canAddToCart(tour: TourDto): boolean {
      return this.isTourist && !this.isCanceled(tour) && !this.isInCart(tour.id);
    }
  }
