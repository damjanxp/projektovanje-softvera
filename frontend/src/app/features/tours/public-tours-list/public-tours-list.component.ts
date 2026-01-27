import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TourService } from '../../../core/services';
import { TourDto, InterestLabels, DifficultyLabels, Interest, Difficulty } from '../../../shared/models';

@Component({
  selector: 'app-public-tours-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './public-tours-list.component.html',
  styleUrl: './public-tours-list.component.css'
})
export class PublicToursListComponent implements OnInit {
  private tourService = inject(TourService);
  
  tours: TourDto[] = [];
  loading = false;
  error: string | null = null;
  sortOrder: 'asc' | 'desc' = 'asc';

  ngOnInit(): void {
    this.loadTours();
  }

  loadTours(): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getPublishedTours(this.sortOrder).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tours = response.data;
        } else {
          this.error = response.error?.message || 'Failed to load tours';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading tours';
      }
    });
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
}
