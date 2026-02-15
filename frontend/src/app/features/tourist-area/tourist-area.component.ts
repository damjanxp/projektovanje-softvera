import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { BalanceService } from '../../core/services/balance.service';

@Component({
selector: 'app-tourist-area',
standalone: true,
imports: [CommonModule, RouterLink, FormsModule],
  template: `
    <div class="area-container">
      <div class="area-header">
        <div class="header-content">
          <span class="header-icon">🏖️</span>
          <h1>Tourist Area</h1>
        </div>
        <p class="welcome-text">Welcome, <span class="username">{{ username }}</span>!</p>
      </div>

      <div class="features-grid">
        <!-- Balance Card -->
        <div class="feature-card balance-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">💰</span>
          </div>
          <div class="card-content">
            <h3>Balance</h3>
            <p class="balance-amount">\${{ balance.toFixed(2) }}</p>
            @if (balanceSuccess) {
              <p class="balance-success">{{ balanceSuccess }}</p>
            }
            @if (balanceError) {
              <p class="balance-error">{{ balanceError }}</p>
            }
            <div class="topup-form">
              <input
                type="number"
                class="topup-input"
                [(ngModel)]="topUpAmount"
                placeholder="Amount ($)"
                min="0.01"
                step="0.01"
              />
              <button class="topup-btn" (click)="addFunds()">Add Funds</button>
            </div>
          </div>
        </div>

        <a routerLink="/tours" class="feature-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">🌍</span>
          </div>
          <div class="card-content">
            <h3>Browse Tours</h3>
            <p>Discover amazing tours from our guides</p>
          </div>
          <div class="card-arrow">→</div>
        </a>

        <a routerLink="/cart" class="feature-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">🛒</span>
          </div>
          <div class="card-content">
            <h3>My Cart</h3>
            <p>View and manage your cart</p>
          </div>
          <div class="card-arrow">→</div>
        </a>

        <a routerLink="/tourist/my-purchases" class="feature-card">
          <div class="card-icon-wrapper">
            <span class="card-icon">⭐</span>
          </div>
          <div class="card-content">
            <h3>Rate & Report</h3>
            <p>Rate your tours and report issues</p>
          </div>
          <div class="card-arrow">→</div>
        </a>
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
      color: var(--color-primary-dark);
      font-weight: 600;
    }

    .features-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: var(--spacing-lg);
      max-width: 1100px;
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

    .feature-card::before {
      content: '';
      position: absolute;
      top: 0;
      left: 0;
      right: 0;
      height: 4px;
      background: linear-gradient(90deg, var(--color-primary) 0%, var(--color-primary-dark) 100%);
      transform: scaleX(0);
      transition: transform var(--transition-normal);
    }

    .feature-card:hover {
      transform: translateY(-6px);
      box-shadow: var(--shadow-hover);
      border-color: var(--color-primary);
    }

    .feature-card:hover::before {
      transform: scaleX(1);
    }

    .card-icon-wrapper {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 80px;
      height: 80px;
      background: linear-gradient(135deg, var(--color-primary-light) 0%, rgba(216, 167, 177, 0.3) 100%);
      border-radius: var(--radius-md);
      margin-bottom: var(--spacing-md);
      transition: transform var(--transition-fast);
    }

    .feature-card:hover .card-icon-wrapper {
      transform: scale(1.1) rotate(5deg);
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

    .card-content p {
      margin: 0;
      color: var(--color-text-secondary);
      font-size: var(--font-size-sm);
      line-height: 1.5;
      text-align: left;
    }

    .card-arrow {
      align-self: flex-end;
      color: var(--color-primary);
      font-size: 1.5rem;
      font-weight: bold;
      opacity: 0;
      transform: translateX(-10px);
      transition: all var(--transition-fast);
    }

    .feature-card:hover .card-arrow {
      opacity: 1;
      transform: translateX(0);
    }

    /* ============ BALANCE CARD ============ */
    .balance-card {
      cursor: default;
    }

    .balance-card:hover {
      transform: none;
      box-shadow: none;
    }

    .balance-card:hover::before {
      transform: scaleX(1);
    }

    .balance-amount {
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--color-secondary-dark);
      margin: var(--spacing-xs) 0 var(--spacing-sm) 0;
    }

    .topup-form {
      display: flex;
      gap: var(--spacing-sm);
      margin-top: var(--spacing-sm);
    }

    .topup-input {
      flex: 1;
      padding: 0.5rem 0.75rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-sm);
      font-size: var(--font-size-sm);
      background: white;
      min-width: 0;
    }

    .topup-input:focus {
      outline: none;
      border-color: var(--color-primary);
      box-shadow: 0 0 0 3px rgba(216, 167, 177, 0.2);
    }

    .topup-btn {
      padding: 0.5rem 1rem;
      background: var(--color-secondary);
      color: var(--color-text-light);
      border: none;
      border-radius: var(--radius-sm);
      font-size: var(--font-size-sm);
      font-weight: 600;
      cursor: pointer;
      white-space: nowrap;
      transition: background var(--transition-fast);
    }

    .topup-btn:hover {
      background: var(--color-secondary-dark);
    }

    .balance-success {
      color: var(--color-success);
      font-size: var(--font-size-sm);
      margin: var(--spacing-xs) 0 0 0;
      font-weight: 500;
    }

    .balance-error {
      color: #8a4a4a;
      font-size: var(--font-size-sm);
      margin: var(--spacing-xs) 0 0 0;
      font-weight: 500;
    }
  `]
})
export class TouristAreaComponent {
  private authService = inject(AuthService);
  private balanceService = inject(BalanceService);

  topUpAmount: number | null = null;
  balanceSuccess: string | null = null;
  balanceError: string | null = null;

  get username(): string {
    return this.authService.getUsername() || 'Tourist';
  }

  get balance(): number {
    return this.balanceService.getBalance();
  }

  addFunds(): void {
    this.balanceSuccess = null;
    this.balanceError = null;

    if (!this.topUpAmount || this.topUpAmount < 0.01) {
      this.balanceError = 'Invalid amount entered.';
      return;
    }

    const newBalance = this.balanceService.addFunds(this.topUpAmount);
    this.balanceSuccess = `Added $${this.topUpAmount.toFixed(2)}. New balance: $${newBalance.toFixed(2)}`;
    this.topUpAmount = null;
  }
}
