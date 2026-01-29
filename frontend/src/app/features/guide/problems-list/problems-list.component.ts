import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { TourService } from '../../../core/services/tour.service';
import { ProblemService } from '../../../core/services/problem.service';
import { ProblemDto, ProblemStatus, ProblemStatusLabels } from '../../../shared/models/problem.model';
import { TourDto } from '../../../shared/models';

interface ProblemWithTour {
  problem: ProblemDto;
  tourName: string;
}

@Component({
  selector: 'app-guide-problems-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './problems-list.component.html',
  styleUrl: './problems-list.component.css'
})
export class GuideProblemsListComponent implements OnInit {
  private tourService = inject(TourService);
  private problemService = inject(ProblemService);

  problems: ProblemWithTour[] = [];
  loading = false;
  error: string | null = null;

  ProblemStatus = ProblemStatus;

  ngOnInit(): void {
    this.loadGuideProblems();
  }

  loadGuideProblems(): void {
    this.loading = true;
    this.error = null;

    // First, get all guide's tours
    this.tourService.getMyTours().subscribe({
      next: (toursResponse) => {
        if (toursResponse.success && toursResponse.data) {
          const tours = toursResponse.data;
          
          if (tours.length === 0) {
            this.loading = false;
            this.problems = [];
            return;
          }

          // For each tour, fetch its problems
          const problemObservables = tours.map(tour =>
            this.problemService.getProblemsForTour(tour.id).pipe(
              map(problemsResponse => ({
                tourName: tour.name,
                problems: problemsResponse.success && problemsResponse.data ? problemsResponse.data : []
              })),
              catchError(() => of({ tourName: tour.name, problems: [] }))
            )
          );

          forkJoin(problemObservables).subscribe({
            next: (results) => {
              this.loading = false;
              // Flatten all problems and attach tour names
              this.problems = results.flatMap(result =>
                result.problems.map(problem => ({
                  problem,
                  tourName: result.tourName
                }))
              );
            },
            error: () => {
              this.loading = false;
              this.error = 'An error occurred while loading problems';
            }
          });
        } else {
          this.loading = false;
          this.error = toursResponse.error?.message || 'Failed to load tours';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading tours';
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

  filterByStatus(status: ProblemStatus): ProblemWithTour[] {
    return this.problems.filter(p => p.problem.status === status);
  }

  get pendingProblems(): ProblemWithTour[] {
    return this.filterByStatus(ProblemStatus.Pending);
  }

  get inReviewProblems(): ProblemWithTour[] {
    return this.filterByStatus(ProblemStatus.InReview);
  }

  get resolvedProblems(): ProblemWithTour[] {
    return this.filterByStatus(ProblemStatus.Resolved);
  }

  get rejectedProblems(): ProblemWithTour[] {
    return this.filterByStatus(ProblemStatus.Rejected);
  }
}
