import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TourService, AuthService, RatingService } from '../../core/services';
import { TourDto, Interest, InterestLabels, DifficultyLabels, Difficulty } from '../../shared/models';

interface TourWithRating extends TourDto {
  averageRating?: number | null;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private tourService = inject(TourService);
  private authService = inject(AuthService);
  private ratingService = inject(RatingService);

  allTours: TourWithRating[] = [];
  youMightLikeTours: TourWithRating[] = [];
  topRatedTours: TourWithRating[] = [];
  loading = false;

  ngOnInit(): void {
    this.loadTours();
  }

  loadTours(): void {
    this.loading = true;
    
    this.tourService.getPublishedTours('desc').subscribe({
      next: (response) => {
        if (response.success && response.data) {
          const tours = response.data;
          
          // Fetch ratings for all tours
          if (tours.length === 0) {
            this.loading = false;
            this.allTours = [];
            this.calculateSections();
            return;
          }

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
              this.allTours = toursWithRatings;
              this.calculateSections();
            },
            error: () => {
              this.loading = false;
              this.allTours = tours.map(tour => ({ ...tour, averageRating: null }));
              this.calculateSections();
            }
          });
        } else {
          this.loading = false;
        }
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private calculateSections(): void {
    // Calculate "You might like" - based on user's interest or random
    this.youMightLikeTours = this.getYouMightLikeTours();
    
    // Calculate "Top rated" - sort by rating desc
    this.topRatedTours = this.getTopRatedTours();
  }

  private getYouMightLikeTours(): TourWithRating[] {
    if (this.allTours.length === 0) return [];

    // If user is Tourist, try to personalize (for now, shuffle randomly)
    // In real app, you could store user interests in profile/token
    const shuffled = [...this.allTours].sort(() => Math.random() - 0.5);
    return shuffled.slice(0, 3);
  }

  private getTopRatedTours(): TourWithRating[] {
    if (this.allTours.length === 0) return [];

    // Sort by rating desc (treat null as 0)
    const sorted = [...this.allTours].sort((a, b) => {
      const ratingA = a.averageRating ?? 0;
      const ratingB = b.averageRating ?? 0;
      return ratingB - ratingA;
    });
    return sorted.slice(0, 3);
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  get isTourist(): boolean {
    return this.authService.getUserRole() === 'Tourist';
  }

  getCategoryLabel(category: Interest): string {
    return InterestLabels[category] || 'Unknown';
  }

  getDifficultyLabel(difficulty: Difficulty): string {
    return DifficultyLabels[difficulty] || 'Unknown';
  }

  formatPrice(price: number): string {
    return `$${price.toFixed(2)}`;
  }

  formatRating(rating: number | null | undefined): string {
    if (rating === null || rating === undefined) return 'No ratings';
    return `⭐ ${rating.toFixed(1)}`;
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
}

