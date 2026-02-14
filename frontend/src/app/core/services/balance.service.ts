import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class BalanceService {
  private platformId = inject(PLATFORM_ID);
  private authService = inject(AuthService);

  private balanceSubject = new BehaviorSubject<number>(0);
  balance$ = this.balanceSubject.asObservable();

  private get isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  private get storageKey(): string {
    const username = this.authService.getUsername();
    return `balance_${username}`;
  }

  constructor() {
    this.loadBalance();
  }

  loadBalance(): void {
    if (!this.isBrowser) {
      return;
    }
    const stored = localStorage.getItem(this.storageKey);
    const balance = stored ? parseFloat(stored) : 0;
    this.balanceSubject.next(this.round(balance));
  }

  getBalance(): number {
    if (!this.isBrowser) {
      return 0;
    }
    const stored = localStorage.getItem(this.storageKey);
    return stored ? this.round(parseFloat(stored)) : 0;
  }

  addFunds(amount: number): number {
    const current = this.getBalance();
    const newBalance = this.round(current + amount);
    this.saveBalance(newBalance);
    return newBalance;
  }

  deductFunds(amount: number): boolean {
    const current = this.getBalance();
    if (amount > current) {
      return false;
    }
    const newBalance = this.round(current - amount);
    this.saveBalance(newBalance);
    return true;
  }

  private saveBalance(balance: number): void {
    if (this.isBrowser) {
      localStorage.setItem(this.storageKey, balance.toFixed(2));
      this.balanceSubject.next(balance);
    }
  }

  private round(value: number): number {
    return Math.round(value * 100) / 100;
  }
}
