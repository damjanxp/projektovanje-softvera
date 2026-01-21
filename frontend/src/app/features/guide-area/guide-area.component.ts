import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-guide-area',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="protected-area guide">
      <div class="area-content">
        <h1>🧭 Guide Area</h1>
        <p>Welcome, {{ username }}!</p>
        <p class="description">This is a protected area for guides only.</p>
        <div class="features">
          <div class="feature-item">
            <h3>My Tours</h3>
            <p>Manage your created tours</p>
          </div>
          <div class="feature-item">
            <h3>Create Tour</h3>
            <p>Create new tour experiences</p>
          </div>
          <div class="feature-item">
            <h3>Problems</h3>
            <p>Handle reported problems</p>
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

    .guide {
      background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
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
    }

    .feature-item h3 {
      color: #11998e;
      margin-bottom: 0.5rem;
    }

    .feature-item p {
      font-size: 0.9rem;
    }
  `]
})
export class GuideAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Guide';
  }
}
