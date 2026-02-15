import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourStatus, TourStatusLabels } from '../../models';
import { ProblemStatus, ProblemStatusLabels } from '../../models/problem.model';

export type BadgeType = 'tour' | 'problem';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="status-badge" [ngClass]="statusClass">
      {{ statusLabel }}
      @if (needsReplacement) {
        <span class="replacement-indicator"> (Replacement Requested)</span>
      }
    </span>
  `,
  styles: [`
    .status-badge {
      display: inline-block;
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
    }
    /* Tour statuses */
    .status-draft {
      background-color: #ffc107;
      color: #212529;
    }
    .status-published {
      background-color: #28a745;
      color: #fff;
    }
    .status-canceled {
      background-color: #dc3545;
      color: #fff;
    }
    /* Problem statuses */
    .status-pending {
      background-color: #ffc107;
      color: #212529;
    }
    .status-in-review {
      background-color: #17a2b8;
      color: #fff;
    }
    .status-resolved {
      background-color: #28a745;
      color: #fff;
    }
    .status-rejected {
      background-color: #dc3545;
      color: #fff;
    }
    .replacement-indicator {
      font-weight: normal;
      text-transform: none;
      font-size: 0.7rem;
    }
  `]
})
export class StatusBadgeComponent {
  @Input() status!: TourStatus | ProblemStatus;
  @Input() type: BadgeType = 'tour';
  @Input() needsReplacement = false;

  get statusLabel(): string {
    if (this.type === 'problem') {
      return ProblemStatusLabels[this.status as ProblemStatus] || 'Unknown';
    }
    return TourStatusLabels[this.status as TourStatus] || 'Unknown';
  }

  get statusClass(): string {
    if (this.type === 'problem') {
      switch (this.status as ProblemStatus) {
        case ProblemStatus.Pending: return 'status-pending';
        case ProblemStatus.InReview: return 'status-in-review';
        case ProblemStatus.Resolved: return 'status-resolved';
        case ProblemStatus.Rejected: return 'status-rejected';
        default: return '';
      }
    }
    
    switch (this.status as TourStatus) {
      case TourStatus.Draft: return 'status-draft';
      case TourStatus.Published: return 'status-published';
      case TourStatus.Canceled: return 'status-canceled';
      default: return '';
    }
  }
}
