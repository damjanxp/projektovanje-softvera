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
      min-height: calc(100vh - 80px);
    }

    .admin {
      background: linear-gradient(135deg, #fc4a1a 0%, #f7b733 100%);
    }

    .area-content {
      max-width: 800px;
      margin: 0 auto;
      background: white;
      padding: 2rem;
      border-radius: 12px;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
    }

    h1 {
      text-align: center;
      color: #333;
      margin-bottom: 0.5rem;
    }

    p {
      text-align: center;
      color: #666;
    }

    .description {
      margin-bottom: 2rem;
    }

    .features {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
    }

    .feature-item {
      background: #f8f9fa;
      padding: 1.5rem;
      border-radius: 8px;
      text-align: center;
      text-decoration: none;
      color: inherit;
      transition: all 0.3s ease;
      cursor: pointer;
      border: 2px solid transparent;
    }

    .feature-item:not(.disabled):hover {
      background: #e9ecef;
      border-color: #fc4a1a;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(252, 74, 26, 0.2);
    }

    .feature-item.disabled {
      opacity: 0.5;
      cursor: not-allowed;
      border: 2px dashed #dee2e6;
    }

    .feature-item h3 {
      color: #fc4a1a;
      margin-bottom: 0.5rem;
    }

    .feature-item.disabled h3 {
      color: #6c757d;
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
