import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-forbidden',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="error-page">
      <div class="error-content">
        <h1>403</h1>
        <h2>Access Forbidden</h2>
        <p>You don't have permission to access this page.</p>
        <div class="error-actions">
          <a routerLink="/" class="btn btn-primary">Go to Home</a>
          <a routerLink="/login" class="btn btn-secondary">Login</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .error-page {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 80px);
      padding: 2rem;
      background-color: #f5f5f5;
    }

    .error-content {
      text-align: center;
      background: white;
      padding: 3rem;
      border-radius: 8px;
      box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    }

    h1 {
      font-size: 6rem;
      color: #dc3545;
      margin: 0;
      line-height: 1;
    }

    h2 {
      font-size: 1.5rem;
      color: #333;
      margin: 1rem 0;
    }

    p {
      color: #666;
      margin-bottom: 2rem;
    }

    .error-actions {
      display: flex;
      gap: 1rem;
      justify-content: center;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border-radius: 4px;
      text-decoration: none;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-primary {
      background-color: #007bff;
      color: white;
    }

    .btn-primary:hover {
      background-color: #0056b3;
    }

    .btn-secondary {
      background-color: #6c757d;
      color: white;
    }

    .btn-secondary:hover {
      background-color: #545b62;
    }
  `]
})
export class ForbiddenComponent {}
