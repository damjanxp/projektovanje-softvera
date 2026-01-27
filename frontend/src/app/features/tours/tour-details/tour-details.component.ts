import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TourService } from '../../../core/services';
import { TourDto, InterestLabels, DifficultyLabels, TourStatusLabels, Interest, Difficulty, TourStatus } from '../../../shared/models';
import { LeafletMapComponent } from '../../../shared/components';

@Component({
  selector: 'app-tour-details',
  standalone: true,
  imports: [CommonModule, RouterLink, LeafletMapComponent],
  templateUrl: './tour-details.component.html',
  styleUrl: './tour-details.component.css'
})
export class TourDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private tourService = inject(TourService);
  
  tour: TourDto | null = null;
  loading = false;
  error: string | null = null;

  ngOnInit(): void {
    const tourId = this.route.snapshot.paramMap.get('id');
    if (tourId) {
      this.loadTour(tourId);
    } else {
      this.error = 'Tour ID not provided';
    }
  }

  loadTour(tourId: string): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getTourById(tourId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tour = response.data;
        } else {
          this.error = response.error?.message || 'Tour not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading the tour';
      }
    });
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
}
