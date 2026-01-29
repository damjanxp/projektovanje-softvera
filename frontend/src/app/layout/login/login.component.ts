import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  template: `
    <div class="login-container">
      <div class="login-card">
        <h2>Login to TourApp</h2>
        
        @if (errorMessage) {
          <div class="error-message">
            {{ errorMessage }}
          </div>
        }

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username" 
              formControlName="username"
              [class.invalid]="loginForm.get('username')?.invalid && loginForm.get('username')?.touched"
              placeholder="Enter your username"
            />
            @if (loginForm.get('username')?.invalid && loginForm.get('username')?.touched) {
              <span class="field-error">Username is required</span>
            }
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password" 
              formControlName="password"
              [class.invalid]="loginForm.get('password')?.invalid && loginForm.get('password')?.touched"
              placeholder="Enter your password"
            />
            @if (loginForm.get('password')?.invalid && loginForm.get('password')?.touched) {
              <span class="field-error">Password is required</span>
            }
          </div>

          <button 
            type="submit" 
            class="login-button"
            [disabled]="loginForm.invalid || isLoading"
          >
            {{ isLoading ? 'Logging in...' : 'Login' }}
          </button>
        </form>

        <div class="login-footer">
          <a routerLink="/">Back to Home</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 60px);
      padding: 2rem;
      background: var(--color-bg-main);
    }

    .login-card {
      background: var(--color-bg-card);
      padding: 2.5rem;
      border-radius: var(--radius-md);
      border: 1px solid var(--color-border);
      box-shadow: var(--shadow-md);
      width: 100%;
      max-width: 420px;
    }

    h2 {
      text-align: center;
      margin-bottom: 1.5rem;
      color: var(--color-text-primary);
      font-size: 1.5rem;
    }

    .error-message {
      background: rgba(199, 123, 123, 0.15);
      color: #8a4a4a;
      padding: 0.875rem 1rem;
      border-radius: var(--radius-sm);
      margin-bottom: 1rem;
      border: 1px solid var(--color-danger);
    }

    .form-group {
      margin-bottom: 1.25rem;
    }

    label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: var(--color-text-primary);
    }

    input {
      width: 100%;
      padding: 0.875rem 1rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      font-size: 1rem;
      transition: border-color 0.2s, box-shadow 0.2s;
      box-sizing: border-box;
      background: white;
    }

    input:focus {
      outline: none;
      border-color: var(--color-primary);
      box-shadow: 0 0 0 3px rgba(216, 167, 177, 0.2);
    }

    input.invalid {
      border-color: var(--color-danger);
    }

    .field-error {
      color: #8a4a4a;
      font-size: 0.875rem;
      margin-top: 0.25rem;
      display: block;
    }

    .login-button {
      width: 100%;
      padding: 1rem;
      background: var(--color-secondary);
      color: var(--color-text-light);
      border: none;
      border-radius: var(--radius-pill);
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
      margin-top: 0.5rem;
    }

    .login-button:hover:not(:disabled) {
      background: var(--color-secondary-dark);
      transform: translateY(-2px);
    }

    .login-button:disabled {
      background: var(--color-border);
      cursor: not-allowed;
    }

    .login-footer {
      text-align: center;
      margin-top: 1.5rem;
    }

    .login-footer a {
      color: var(--color-primary-dark);
      text-decoration: none;
      transition: color 0.2s;
    }

    .login-footer a:hover {
      color: var(--color-primary);
      text-decoration: underline;
    }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;

  constructor() {
    // Redirect if already authenticated
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
    }

    this.loginForm = this.fb.group({
      username: ['', [Validators.required]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const { username, password } = this.loginForm.value;

    this.authService.login(username, password).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = response.error?.message || 'Login failed. Please try again.';
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'An unexpected error occurred. Please try again.';
      }
    });
  }
}
