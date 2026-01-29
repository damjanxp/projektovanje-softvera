import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
selector: 'app-guide-area',
standalone: true,
imports: [CommonModule, RouterLink],
  template: `
    <div class="protected-area guide">
      <div class="area-content">
        <h1>🧭 Guide Area</h1>
        <p>Welcome, {{ username }}!</p>
        <p class="description">This is a protected area for guides only.</p>
        <div class="features">
          <a routerLink="/guide/tours" class="feature-item">
            <h3>📚 My Tours</h3>
            <p>Manage your created tours</p>
          </a>
          <a routerLink="/guide/tours/create" class="feature-item">
            <h3>➕ Create Tour</h3>
            <p>Create new tour experiences</p>
          </a>
          <a routerLink="/guide/problems" class="feature-item">
            <h3>🚨 Problems</h3>
            <p>Handle reported problems</p>
          </a>
          <a routerLink="/guide/replacements" class="feature-item">
            <h3>🔄 Replacements</h3>
            <p>View available replacement tours</p>
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

    .guide {
      background: var(--color-bg-main);
    }

    .area-content {
      max-width: 1000px;
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
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
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
      border-color: var(--color-secondary);
      transform: translateY(-4px);
      box-shadow: 0 6px 16px rgba(163, 177, 138, 0.25);
    }

    .feature-item h3 {
      color: var(--color-secondary-dark);
      margin-bottom: 0.5rem;
      font-size: 1.1rem;
    }

    .feature-item p {
      font-size: 0.9rem;
      margin: 0;
    }
  `]
})
export class GuideAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Guide';
  }
}
