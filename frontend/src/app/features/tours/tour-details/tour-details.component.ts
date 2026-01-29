import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TourService, CartService, AuthService, RatingService } from '../../../core/services';
import { ApiResponse, TourDto, InterestLabels, DifficultyLabels, TourStatusLabels, Interest, Difficulty, TourStatus } from '../../../shared/models';
import { RatingDto } from '../../../shared/models/rating.model';
import { LeafletMapComponent, StatusBadgeComponent } from '../../../shared/components';

@Component({
  selector: 'app-tour-details',
  standalone: true,
  imports: [CommonModule, RouterLink, LeafletMapComponent, StatusBadgeComponent],
  templateUrl: './tour-details.component.html',
  styleUrl: './tour-details.component.css'
})
export class TourDetailsComponent implements OnInit {
private route = inject(ActivatedRoute);
private tourService = inject(TourService);
private cartService = inject(CartService);
private authService = inject(AuthService);
private ratingService = inject(RatingService);
  
tour: TourDto | null = null;
ratings: RatingDto[] = [];
averageRating: number | null = null;
loading = false;
loadingRatings = false;
error: string | null = null;
addingToCart = false;

// Expose Math to template
Math = Math;

  ngOnInit(): void {
    const tourId = this.route.snapshot.paramMap.get('id');
    if (tourId) {
      this.loadTour(tourId);
      this.loadRatings(tourId);
      if (this.isTourist) {
        this.loadCartItems();
      }
    } else {
      this.error = 'Tour ID not provided';
    }
  }

  loadTour(tourId: string): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getTourById(tourId).subscribe({
      next: (response: ApiResponse<TourDto>) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tour = response.data;
        } else {
          this.error = response.error?.message || 'Tour not found';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading the tour';
      }
    });
  }

  loadRatings(tourId: string): void {
    this.loadingRatings = true;

    // Load ratings list
    this.ratingService.getRatingsForTour(tourId).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.ratings = response.data;
        }
      },
      error: () => {
        // Silently fail - ratings are optional
      }
    });

    // Load average rating
      this.ratingService.getAverageRatingForTour(tourId).subscribe({
        next: (response) => {
          this.loadingRatings = false;
          if (response.success) {
            this.averageRating = response.data ?? null;
          }
        },
        error: () => {
          this.loadingRatings = false;
        }
      });
    }

  loadCartItems(): void {
    this.cartService.getCart().subscribe();
  }

  addToCart(): void {
    if (!this.tour) return;
    
    this.addingToCart = true;
    
    this.cartService.addToCart(this.tour.id).subscribe({
      next: (response: ApiResponse<void>) => {
        this.addingToCart = false;
        if (!response.success) {
          this.error = response.error?.message || 'Failed to add to cart';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.addingToCart = false;
        this.error = err.error?.error?.message || 'An error occurred while adding to cart';
      }
    });
  }

  get isInCart(): boolean {
    return this.tour ? this.cartService.isInCartSync(this.tour.id) : false;
  }

  get isTourist(): boolean {
    return this.authService.getUserRole() === 'Tourist';
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  getCategoryLabel(category: Interest): string {
    return InterestLabels[category] || 'Unknown';
  }

  getDifficultyLabel(difficulty: Difficulty): string {
    return DifficultyLabels[difficulty] || 'Unknown';
  }

  getStatusLabel(status: TourStatus): string {
    return TourStatusLabels[status] || 'Unknown';
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatPrice(price: number): string {
    return `$${price.toFixed(2)}`;
  }

  get sortedKeyPoints() {
      if (!this.tour) return [];
      return [...this.tour.keyPoints].sort((a, b) => a.order - b.order);
    }

    get isCanceled(): boolean {
      return this.tour?.status === TourStatus.Canceled;
    }

    get needsReplacement(): boolean {
      return this.tour?.needsReplacement === true;
    }

    get canAddToCart(): boolean {
        return this.isTourist && !this.isCanceled && !this.needsReplacement && !this.isInCart;
      }

      getStarArray(rating: number): boolean[] {
        return Array(5).fill(false).map((_, index) => index < rating);
      }

      formatRatingDate(dateString: string): string {
        return new Date(dateString).toLocaleDateString('en-US', {
          year: 'numeric',
          month: 'short',
          day: 'numeric'
        });
      }
    }
