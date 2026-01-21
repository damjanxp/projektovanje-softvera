import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-tourist-area',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="protected-area tourist">
      <div class="area-content">
        <h1>🏖️ Tourist Area</h1>
        <p>Welcome, {{ username }}!</p>
        <p class="description">This is a protected area for tourists only.</p>
        <div class="features">
          <div class="feature-item">
            <h3>Browse Tours</h3>
            <p>Discover amazing tours from our guides</p>
          </div>
          <div class="feature-item">
            <h3>My Purchases</h3>
            <p>View your purchased tours</p>
          </div>
          <div class="feature-item">
            <h3>Rate Tours</h3>
            <p>Share your experience with others</p>
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

    .tourist {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
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
      color: #667eea;
      margin-bottom: 0.5rem;
    }

    .feature-item p {
      font-size: 0.9rem;
    }
  `]
})
export class TouristAreaComponent {
  private authService = inject(AuthService);

  get username(): string {
    return this.authService.getUsername() || 'Tourist';
  }
}
