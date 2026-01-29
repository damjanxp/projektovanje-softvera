import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProblemService } from '../../../core/services/problem.service';
import { ProblemDto, ProblemStatus, ProblemStatusLabels } from '../../../shared/models/problem.model';

@Component({
  selector: 'app-problems-review',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './problems-review.component.html',
  styleUrl: './problems-review.component.css'
})
export class ProblemsReviewComponent implements OnInit {
  private problemService = inject(ProblemService);

  problems: ProblemDto[] = [];
  loading = false;
  error: string | null = null;

  ProblemStatus = ProblemStatus;

  ngOnInit(): void {
    this.loadProblemsInReview();
  }

  loadProblemsInReview(): void {
    this.loading = true;
    this.error = null;

    this.problemService.getProblemsInReview().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.problems = response.data;
        } else {
          this.error = response.error?.message || 'Failed to load problems in review';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading problems';
      }
    });
  }

  getStatusLabel(status: ProblemStatus): string {
    return ProblemStatusLabels[status] || 'Unknown';
  }

  getStatusClass(status: ProblemStatus): string {
    switch (status) {
      case ProblemStatus.Pending: return 'status-pending';
      case ProblemStatus.Resolved: return 'status-resolved';
      case ProblemStatus.InReview: return 'status-in-review';
      case ProblemStatus.Rejected: return 'status-rejected';
      default: return '';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
