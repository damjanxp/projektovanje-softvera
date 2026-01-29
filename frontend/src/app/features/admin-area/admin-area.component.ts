import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
selector: 'app-admin-area',
standalone: true,
imports: [CommonModule, RouterLink],
  template: `
    <div class="protected-area admin">
      <div class="area-content">
        <h1>⚙️ Admin Area</h1>
        <p>Welcome, {{ username }}!</p>
        <p class="description">This is a protected area for administrators only.</p>
        <div class="features">
          <a routerLink="/admin/blocked-users" class="feature-item">
            <h3>🔒 Blocked Users</h3>
            <p>View and unblock users</p>
          </a>
          <a routerLink="/admin/problems-review" class="feature-item">
            <h3>📋 Problem Review</h3>
            <p>Review escalated problems</p>
          </a>
          <div class="feature-item disabled">
            <h3>👥 User Management</h3>
            <p>Coming soon</p>
          </div>
          <div class="feature-item disabled">
            <h3>📊 System Stats</h3>
            <p>Coming soon</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .protected-area {
      padding: 2rem;
      min-height: calc(100vh - 60px);
    }

    .admin {
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

    .feature-item:not(.disabled):hover {
      background: rgba(255, 255, 255, 0.9);
      border-color: var(--color-accent);
      transform: translateY(-4px);
      box-shadow: 0 6px 16px rgba(255, 215, 0, 0.25);
    }

    .feature-item.disabled {
      opacity: 0.5;
      cursor: not-allowed;
      border: 2px dashed var(--color-border);
    }

    .feature-item h3 {
      color: var(--color-accent-dark);
      margin-bottom: 0.5rem;
      font-size: 1.1rem;
    }

    .feature-item.disabled h3 {
      color: var(--color-text-muted);
    }

    .feature-item p {
      font-size: 0.9rem;
      margin: 0;
    }
  `]
})
export class AdminAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Admin';
  }
}
