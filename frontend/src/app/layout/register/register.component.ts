import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

interface InterestOption {
  value: number;
  label: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  template: `
    <div class="register-container">
      <div class="register-card">
        <h2>Register as Tourist</h2>
        
        @if (errorMessage) {
          <div class="error-message">
            {{ errorMessage }}
          </div>
        }

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <div class="form-row">
            <div class="form-group">
              <label for="firstName">First Name</label>
              <input 
                type="text" 
                id="firstName" 
                formControlName="firstName"
                [class.invalid]="registerForm.get('firstName')?.invalid && registerForm.get('firstName')?.touched"
                placeholder="Enter your first name"
              />
              @if (registerForm.get('firstName')?.invalid && registerForm.get('firstName')?.touched) {
                <span class="field-error">First name is required</span>
              }
            </div>

            <div class="form-group">
              <label for="lastName">Last Name</label>
              <input 
                type="text" 
                id="lastName" 
                formControlName="lastName"
                [class.invalid]="registerForm.get('lastName')?.invalid && registerForm.get('lastName')?.touched"
                placeholder="Enter your last name"
              />
              @if (registerForm.get('lastName')?.invalid && registerForm.get('lastName')?.touched) {
                <span class="field-error">Last name is required</span>
              }
            </div>
          </div>

          <div class="form-group">
            <label for="username">Username</label>
            <input 
              type="text" 
              id="username" 
              formControlName="username"
              [class.invalid]="registerForm.get('username')?.invalid && registerForm.get('username')?.touched"
              placeholder="Choose a username"
            />
            @if (registerForm.get('username')?.invalid && registerForm.get('username')?.touched) {
              <span class="field-error">Username is required</span>
            }
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input 
              type="email" 
              id="email" 
              formControlName="email"
              [class.invalid]="registerForm.get('email')?.invalid && registerForm.get('email')?.touched"
              placeholder="Enter your email address"
            />
            @if (registerForm.get('email')?.invalid && registerForm.get('email')?.touched) {
              <span class="field-error">
                @if (registerForm.get('email')?.errors?.['required']) {
                  Email is required
                } @else if (registerForm.get('email')?.errors?.['email']) {
                  Please enter a valid email address
                }
              </span>
            }
          </div>

          <div class="form-group">
            <label for="password">Password</label>
            <input 
              type="password" 
              id="password" 
              formControlName="password"
              [class.invalid]="registerForm.get('password')?.invalid && registerForm.get('password')?.touched"
              placeholder="Choose a password (min. 6 characters)"
            />
            @if (registerForm.get('password')?.invalid && registerForm.get('password')?.touched) {
              <span class="field-error">Password must be at least 6 characters</span>
            }
          </div>

          <div class="form-group">
            <label for="confirmPassword">Confirm Password</label>
            <input 
              type="password" 
              id="confirmPassword" 
              formControlName="confirmPassword"
              [class.invalid]="registerForm.get('confirmPassword')?.invalid && registerForm.get('confirmPassword')?.touched"
              placeholder="Confirm your password"
            />
            @if (registerForm.get('confirmPassword')?.invalid && registerForm.get('confirmPassword')?.touched) {
              <span class="field-error">
                @if (registerForm.get('confirmPassword')?.errors?.['required']) {
                  Please confirm your password
                } @else if (registerForm.get('confirmPassword')?.errors?.['passwordMismatch']) {
                  Passwords do not match
                }
              </span>
            }
          </div>

          <div class="form-group">
            <label>Interests (select at least one)</label>
            <div class="interests-grid">
              @for (interest of interests; track interest.value) {
                <label class="interest-checkbox">
                  <input 
                    type="checkbox" 
                    [value]="interest.value"
                    [checked]="selectedInterests.has(interest.value)"
                    (change)="onInterestChange(interest.value, $event)"
                  />
                  <span>{{ interest.label }}</span>
                </label>
              }
            </div>
            @if (showInterestsError) {
              <span class="field-error">Please select at least one interest</span>
            }
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input 
                type="checkbox" 
                formControlName="wantsRecommendations"
              />
              <span>I want to receive personalized tour recommendations</span>
            </label>
          </div>

          <button 
            type="submit" 
            class="register-button"
            [disabled]="registerForm.invalid || isLoading || selectedInterests.size === 0"
          >
            {{ isLoading ? 'Registering...' : 'Register' }}
          </button>
        </form>

        <div class="register-footer">
          <span>Already have an account?</span>
          <a routerLink="/login">Login here</a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .register-container {
      display: flex;
      justify-content: center;
      align-items: center;
      min-height: calc(100vh - 60px);
      padding: 2rem;
      background: var(--color-bg-main);
    }

    .register-card {
      background: var(--color-bg-card);
      padding: 2.5rem;
      border-radius: var(--radius-md);
      border: 1px solid var(--color-border);
      box-shadow: var(--shadow-md);
      width: 100%;
      max-width: 600px;
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

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1rem;
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

    input[type="text"],
    input[type="email"],
    input[type="password"] {
      width: 100%;
      padding: 0.875rem 1rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      font-size: 1rem;
      transition: border-color 0.2s, box-shadow 0.2s;
      box-sizing: border-box;
      background: white;
    }

    input[type="text"]:focus,
    input[type="email"]:focus,
    input[type="password"]:focus {
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

    .interests-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 0.75rem;
      margin-top: 0.5rem;
    }

    .interest-checkbox {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      padding: 0.5rem;
      border-radius: var(--radius-sm);
      transition: background-color 0.2s;
    }

    .interest-checkbox:hover {
      background-color: rgba(216, 167, 177, 0.1);
    }

    .interest-checkbox input[type="checkbox"] {
      width: auto;
      cursor: pointer;
    }

    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      cursor: pointer;
      font-weight: normal;
    }

    .checkbox-label input[type="checkbox"] {
      width: auto;
      cursor: pointer;
    }

    .register-button {
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

    .register-button:hover:not(:disabled) {
      background: var(--color-secondary-dark);
      transform: translateY(-2px);
    }

    .register-button:disabled {
      background: var(--color-border);
      cursor: not-allowed;
    }

    .register-footer {
      text-align: center;
      margin-top: 1.5rem;
      display: flex;
      gap: 0.5rem;
      justify-content: center;
      align-items: center;
    }

    .register-footer a {
      color: var(--color-primary-dark);
      text-decoration: none;
      transition: color 0.2s;
      font-weight: 500;
    }

    .register-footer a:hover {
      color: var(--color-primary);
      text-decoration: underline;
    }

    @media (max-width: 640px) {
      .form-row {
        grid-template-columns: 1fr;
      }

      .interests-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  registerForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;
  selectedInterests = new Set<number>();
  showInterestsError = false;

  interests: InterestOption[] = [
    { value: 0, label: 'Nature' },
    { value: 1, label: 'Art' },
    { value: 2, label: 'Sport' },
    { value: 3, label: 'Shopping' },
    { value: 4, label: 'Food' }
  ];

  constructor() {
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/']);
    }

    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      username: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      wantsRecommendations: [false]
    });

    this.registerForm.get('confirmPassword')?.setValidators([
      Validators.required,
      this.passwordMatchValidator.bind(this)
    ]);
  }

  passwordMatchValidator(control: any) {
    const password = this.registerForm?.get('password')?.value;
    const confirmPassword = control.value;
    
    if (password !== confirmPassword) {
      return { passwordMismatch: true };
    }
    return null;
  }

  onInterestChange(value: number, event: Event): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      this.selectedInterests.add(value);
    } else {
      this.selectedInterests.delete(value);
    }
    this.showInterestsError = false;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    if (this.selectedInterests.size === 0) {
      this.showInterestsError = true;
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    const formValue = this.registerForm.value;
    const registerData = {
      username: formValue.username,
      password: formValue.password,
      email: formValue.email,
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      interests: Array.from(this.selectedInterests),
      wantsRecommendations: formValue.wantsRecommendations
    };

    this.authService.register(registerData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigate(['/']);
        } else {
          this.errorMessage = response.error?.message || 'Registration failed. Please try again.';
        }
      },
      error: () => {
        this.isLoading = false;
        this.errorMessage = 'An unexpected error occurred. Please try again.';
      }
    });
  }
}
