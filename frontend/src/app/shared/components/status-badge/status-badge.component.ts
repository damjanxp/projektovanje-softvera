import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TourStatus, TourStatusLabels } from '../../models';

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
    .replacement-indicator {
      font-weight: normal;
      text-transform: none;
      font-size: 0.7rem;
    }
  `]
})
export class StatusBadgeComponent {
  @Input() status!: TourStatus;
  @Input() needsReplacement = false;

  get statusLabel(): string {
    return TourStatusLabels[this.status] || 'Unknown';
  }

  get statusClass(): string {
    switch (this.status) {
      case TourStatus.Draft: return 'status-draft';
      case TourStatus.Published: return 'status-published';
      case TourStatus.Canceled: return 'status-canceled';
      default: return '';
    }
  }
}