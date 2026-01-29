import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ReplacementService } from '../../../core/services/replacement.service';
import { ReplacementTourDto } from '../../../shared/models/replacement.model';
import { InterestLabels, DifficultyLabels, Interest, Difficulty } from '../../../shared/models';

@Component({
  selector: 'app-replacement-board',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './replacement-board.component.html',
  styleUrl: './replacement-board.component.css'
})
export class ReplacementBoardComponent implements OnInit {
  private replacementService = inject(ReplacementService);

  tours: ReplacementTourDto[] = [];
  loading = false;
  error: string | null = null;
  takingOver: { [tourId: string]: boolean } = {};
  successMessage: string | null = null;

  ngOnInit(): void {
    this.loadTours();
  }

  loadTours(): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;

    this.replacementService.getReplacementTours().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tours = response.data;
        } else {
          this.error = response.error?.message || 'Failed to load replacement tours';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading tours';
      }
    });
    }

    takeOver(tour: ReplacementTourDto): void {
    if (this.takingOver[tour.id]) return;

    this.takingOver[tour.id] = true;
    this.error = null;
    this.successMessage = null;

    this.replacementService.takeOverTour(tour.id).subscribe({
      next: (response) => {
        this.takingOver[tour.id] = false;
        if (response.success) {
          this.tours = this.tours.filter(t => t.id !== tour.id);
          this.successMessage = `Successfully took over tour "${tour.name}"!`;
        } else {
          this.error = response.error?.message || 'Failed to take over tour';
        }
      },
      error: (err) => {
        this.takingOver[tour.id] = false;
        this.error = err.error?.error?.message || 'An error occurred while taking over the tour';
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
