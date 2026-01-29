import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProblemService } from '../../../core/services/problem.service';
import { AuthService } from '../../../core/services/auth.service';
import { ProblemDto, ProblemEventDto, ProblemStatus, ProblemStatusLabels } from '../../../shared/models/problem.model';

@Component({
  selector: 'app-problem-details',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './problem-details.component.html',
  styleUrl: './problem-details.component.css'
})
export class ProblemDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private problemService = inject(ProblemService);
  private authService = inject(AuthService);

  problem: ProblemDto | null = null;
  events: ProblemEventDto[] = [];
  loading = false;
  eventsLoading = false;
  actionLoading = false;
  error: string | null = null;
  actionError: string | null = null;
  actionSuccess: string | null = null;

  ProblemStatus = ProblemStatus;

  ngOnInit(): void {
    const problemId = this.route.snapshot.paramMap.get('id');
    if (problemId) {
      this.loadProblem(problemId);
      this.loadEvents(problemId);
    } else {
      this.error = 'Problem ID not provided';
    }
  }

  loadProblem(problemId: string): void {
    this.loading = true;
    this.error = null;

    this.problemService.getProblem(problemId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.problem = response.data;
        } else {
          this.error = response.error?.message || 'Problem not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading the problem';
      }
    });
  }

  loadEvents(problemId: string): void {
    this.eventsLoading = true;

    this.problemService.getProblemEvents(problemId).subscribe({
      next: (response) => {
        this.eventsLoading = false;
        if (response.success && response.data) {
          this.events = response.data;
        }
      },
      error: () => {
        this.eventsLoading = false;
      }
    });
  }

  get userRole(): string | null {
    return this.authService.getUserRole();
  }

  get isGuide(): boolean {
    return this.userRole === 'Guide';
  }

  get isAdmin(): boolean {
    return this.userRole === 'Admin';
  }

  get canResolve(): boolean {
    return this.isGuide && this.problem?.status === ProblemStatus.Pending;
  }

  get canSendToReview(): boolean {
    return this.isGuide && this.problem?.status === ProblemStatus.Pending;
  }

  get canReject(): boolean {
    return this.isAdmin && this.problem?.status === ProblemStatus.InReview;
  }

  get canReopen(): boolean {
    return this.isAdmin && this.problem?.status === ProblemStatus.InReview;
  }

  resolveProblem(): void {
    if (!this.problem) return;
    this.performAction(() => this.problemService.resolveProblem(this.problem!.id), 'Problem resolved successfully!');
  }

  sendToReview(): void {
    if (!this.problem) return;
    this.performAction(() => this.problemService.sendToReview(this.problem!.id), 'Problem sent to review!');
  }

  rejectProblem(): void {
    if (!this.problem) return;
    this.performAction(() => this.problemService.rejectProblem(this.problem!.id), 'Problem rejected!');
  }

  reopenProblem(): void {
    if (!this.problem) return;
    this.performAction(() => this.problemService.reopenProblem(this.problem!.id), 'Problem reopened!');
  }

  private performAction(action: () => ReturnType<typeof this.problemService.resolveProblem>, successMessage: string): void {
    this.actionLoading = true;
    this.actionError = null;
    this.actionSuccess = null;

    action().subscribe({
      next: (response) => {
        this.actionLoading = false;
        if (response.success && response.data) {
          this.problem = response.data;
          this.actionSuccess = successMessage;
          this.loadEvents(this.problem.id);
        } else {
          this.actionError = response.error?.message || 'Action failed';
        }
      },
      error: (err) => {
        this.actionLoading = false;
        this.actionError = err.error?.error?.message || 'An error occurred';
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
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
