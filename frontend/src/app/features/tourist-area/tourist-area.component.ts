import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
selector: 'app-tourist-area',
standalone: true,
imports: [CommonModule, RouterLink],
  template: `
    <div class="protected-area tourist">
      <div class="area-content">
        <h1>🏖️ Tourist Area</h1>
        <p>Welcome, {{ username }}!</p>
        <p class="description">This is a protected area for tourists only.</p>
        <div class="features">
          <a routerLink="/tours" class="feature-item">
            <h3>🌍 Browse Tours</h3>
            <p>Discover amazing tours from our guides</p>
          </a>
          <a routerLink="/cart" class="feature-item">
            <h3>🛒 My Cart</h3>
            <p>View and manage your cart</p>
          </a>
          <a routerLink="/tourist/my-purchases" class="feature-item">
            <h3>⭐ Rate & Report</h3>
            <p>Rate your tours and report issues</p>
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .protected-area {
      padding: 2rem;
      min-height: calc(100vh - 60px);
    }

    .tourist {
      background: var(--color-bg-main);
    }

    .area-content {
      max-width: 900px;
      margin: 0 auto;
      background: var(--color-bg-card);
      padding: 2.5rem;
      border-radius: var(--radius-md);
      border: 1px solid var(--color-border);
      box-shadow: var(--shadow-lg);
    }

    h1 {
      text-align: center;
      color: var(--color-text-primary);
      margin-bottom: 0.5rem;
      font-size: 2rem;
    }

    p {
      text-align: center;
      color: var(--color-text-secondary);
    }

    .description {
      margin-bottom: 2rem;
    }

    .features {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 1.25rem;
    }

    .feature-item {
      background: rgba(255, 255, 255, 0.7);
      padding: 1.75rem;
      border-radius: var(--radius-md);
      text-align: center;
      text-decoration: none;
      color: inherit;
      transition: all 0.25s ease;
      cursor: pointer;
      border: 2px solid transparent;
    }

    .feature-item:hover {
      background: rgba(255, 255, 255, 0.9);
      border-color: var(--color-primary);
      transform: translateY(-4px);
      box-shadow: 0 6px 16px rgba(216, 167, 177, 0.25);
    }

    .feature-item h3 {
      color: var(--color-primary-dark);
      margin-bottom: 0.5rem;
      font-size: 1.1rem;
    }

    .feature-item p {
      font-size: 0.9rem;
      margin: 0;
    }
  `]
})
export class TouristAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Tourist';
  }
}
