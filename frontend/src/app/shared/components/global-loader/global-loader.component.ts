import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../../core/services/loading.service';

@Component({
  selector: 'app-global-loader',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (loadingService.isLoading$ | async) {
      <div class="loader-overlay">
        <div class="loader-bar"></div>
      </div>
    }
  `,
  styles: [`
    .loader-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 9999;
      pointer-events: none;
    }

    .loader-bar {
      height: 3px;
      background: linear-gradient(90deg, #007bff, #00d4ff, #007bff);
      background-size: 200% 100%;
      animation: loading 1.5s ease-in-out infinite;
    }

    @keyframes loading {
      0% {
        background-position: 200% 0;
      }
      100% {
        background-position: -200% 0;
      }
    }
  `]
})
export class GlobalLoaderComponent {
  loadingService = inject(LoadingService);
}
