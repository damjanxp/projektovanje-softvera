import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
selector: 'app-admin-area',
standalone: true,
imports: [CommonModule, RouterLink],
  template: `
    <div class="area-container">
      <div class="area-header">
        <div class="header-content">
          <span class="header-icon">⚙️</span>
          <h1>Admin Area</h1>
        </div>
        <p class="welcome-text">Welcome, <span class="username">{{ username }}</span>!</p>
      </div>

      <div class="features-grid">
        <a routerLink="/admin/blocked-users" class="feature-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">🔒</span>
          </div>
          <div class="card-content">
            <h3>Blocked Users</h3>
            <p>View and unblock users</p>
          </div>
          <div class="card-arrow">→</div>
        </a>

        <a routerLink="/admin/problems-review" class="feature-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">📋</span>
          </div>
          <div class="card-content">
            <h3>Problem Review</h3>
            <p>Review escalated problems</p>
          </div>
          <div class="card-arrow">→</div>
        </a>

        <div class="feature-card disabled">
          <div class="card-icon-wrapper">
            <span class="card-icon">👥</span>
          </div>
          <div class="card-content">
            <h3>User Management</h3>
            <p>Coming soon</p>
          </div>
        </div>

        <div class="feature-card disabled">
          <div class="card-icon-wrapper">
            <span class="card-icon">📊</span>
          </div>
          <div class="card-content">
            <h3>System Stats</h3>
            <p>Coming soon</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .area-container {
      max-width: 1280px;
      margin: 0 auto;
      padding: var(--spacing-xl) var(--spacing-lg);
      min-height: calc(100vh - 200px);
    }

    .area-header {
      text-align: center;
      margin-bottom: var(--spacing-xxl);
    }

    .header-content {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: var(--spacing-md);
      margin-bottom: var(--spacing-sm);
    }

    .header-icon {
      font-size: 3rem;
      line-height: 1;
    }

    h1 {
      margin: 0;
      color: var(--color-text-primary);
      font-size: var(--font-size-xxxl);
      font-weight: 700;
    }

    .welcome-text {
      color: var(--color-text-secondary);
      font-size: var(--font-size-lg);
      margin: 0;
    }

    .username {
      color: var(--color-accent-dark);
      font-weight: 600;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--spacing-lg);
      max-width: 1200px;
      margin: 0 auto;
    }

    @media (max-width: 1024px) {
      .features-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    @media (max-width: 640px) {
      .features-grid {
        grid-template-columns: 1fr;
      }
    }

    .feature-card {
      display: flex;
      flex-direction: column;
      background: var(--color-bg-card);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      padding: var(--spacing-xl);
      text-decoration: none;
      color: inherit;
      transition: all var(--transition-normal);
      position: relative;
      overflow: hidden;
    }

    .feature-card:not(.disabled)::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 4px;
      background: linear-gradient(90deg, var(--color-accent) 0%, var(--color-accent-dark) 100%);
      transform: scaleX(0);
      transition: transform var(--transition-normal);
    }

    .feature-card:not(.disabled):hover {
      transform: translateY(-6px);
      box-shadow: var(--shadow-hover);
      border-color: var(--color-accent);
    }

    .feature-card:not(.disabled):hover::before {
      transform: scaleX(1);
    }

    .feature-card.disabled {
      opacity: 0.6;
      cursor: not-allowed;
      border: 1px dashed var(--color-border);
    }

    .card-icon-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 80px;
      height: 80px;
      background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.1) 100%);
      border-radius: var(--radius-md);
      margin-bottom: var(--spacing-md);
      transition: transform var(--transition-fast);
    }

    .feature-card:not(.disabled):hover .card-icon-wrapper {
      transform: scale(1.1) rotate(5deg);
    }

    .feature-card.disabled .card-icon-wrapper {
      background: rgba(0, 0, 0, 0.05);
    }

    .card-icon {
      font-size: 2.5rem;
      line-height: 1;
    }

    .card-content {
      flex: 1;
    }

    .card-content h3 {
      margin: 0 0 var(--spacing-xs) 0;
      color: var(--color-text-primary);
      font-size: var(--font-size-lg);
      font-weight: 600;
    }

    .feature-card.disabled .card-content h3 {
      color: var(--color-text-muted);
    }

    .card-content p {
      margin: 0;
      color: var(--color-text-secondary);
      font-size: var(--font-size-sm);
      line-height: 1.5;
      text-align: left;
    }

    .feature-card.disabled .card-content p {
      color: var(--color-text-muted);
      font-style: italic;
    }

    .card-arrow {
      align-self: flex-end;
      color: var(--color-accent);
      font-size: 1.5rem;
      font-weight: bold;
      opacity: 0;
      transform: translateX(-10px);
      transition: all var(--transition-fast);
    }

    .feature-card:not(.disabled):hover .card-arrow {
      opacity: 1;
      transform: translateX(0);
    }
  `]
})
export class AdminAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Admin';
  }
}
