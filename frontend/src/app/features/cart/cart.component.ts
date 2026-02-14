import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService, PurchaseService, BalanceService } from '../../core/services';
import { ApiResponse, CartDto, CartItemDto, PurchaseResponse, BonusPointsDto } from '../../shared/models';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit {
  private cartService = inject(CartService);
  private purchaseService = inject(PurchaseService);
  private balanceService = inject(BalanceService);
  private router = inject(Router);

  cart: CartDto | null = null;
  bonusPoints = 0;
  useBonus = false;
  
  loading = false;
  loadingBonus = false;
  removingItemId: string | null = null;
  purchasing = false;
  
  error: string | null = null;
  successMessage: string | null = null;

  ngOnInit(): void {
    this.loadCart();
    this.loadBonusPoints();
  }

  loadCart(): void {
    this.loading = true;
    this.error = null;
    
    this.cartService.getCart().subscribe({
      next: (response: ApiResponse<CartDto>) => {
        this.loading = false;
        if (response.success && response.data) {
          this.cart = response.data;
        } else {
          this.error = response.error?.message || 'Failed to load cart';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading cart';
      }
    });
  }

  loadBonusPoints(): void {
    this.loadingBonus = true;
    
    this.purchaseService.getBonusPoints().subscribe({
      next: (response: ApiResponse<BonusPointsDto>) => {
        this.loadingBonus = false;
        if (response.success && response.data) {
          this.bonusPoints = response.data.points;
        }
      },
      error: () => {
        this.loadingBonus = false;
        this.bonusPoints = 0;
      }
    });
  }

  removeItem(tourId: string): void {
    this.removingItemId = tourId;
    this.error = null;
    
    this.cartService.removeFromCart(tourId).subscribe({
      next: (response: ApiResponse<void>) => {
        this.removingItemId = null;
        if (response.success) {
          this.loadCart();
        } else {
          this.error = response.error?.message || 'Failed to remove item';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.removingItemId = null;
        this.error = err.error?.error?.message || 'An error occurred while removing item';
      }
    });
  }

  confirmPurchase(): void {
    if (!this.cart || this.cart.items.length === 0) {
      return;
    }

    const finalPrice = this.displayedPrice;

    // Check balance before proceeding
    if (finalPrice > 0 && !this.balanceService.deductFunds(finalPrice)) {
      this.error = 'Insufficient funds. Please top up your balance.';
      return;
    }

    this.purchasing = true;
    this.error = null;
    this.successMessage = null;
    
    this.purchaseService.confirmPurchase(this.useBonus).subscribe({
      next: (response: ApiResponse<PurchaseResponse>) => {
        this.purchasing = false;
        if (response.success && response.data) {
          const { finalPrice: serverPrice, usedBonusPoints } = response.data;
          this.successMessage = `Purchase successful! Final price: $${serverPrice.toFixed(2)}`;
          if (usedBonusPoints > 0) {
            this.successMessage += ` (Used ${usedBonusPoints} bonus points)`;
          }
          this.cartService.clearLocalCart();
          setTimeout(() => {
            this.router.navigate(['/']);
          }, 2000);
        } else {
          // Rollback balance on backend error
          if (finalPrice > 0) {
            this.balanceService.addFunds(finalPrice);
          }
          this.error = response.error?.message || 'Purchase failed';
        }
      },
      error: (err: { error?: { error?: { message?: string } } }) => {
        this.purchasing = false;
        // Rollback balance on backend error
        if (finalPrice > 0) {
          this.balanceService.addFunds(finalPrice);
        }
        this.error = err.error?.error?.message || 'An error occurred during purchase';
      }
    });
  }

  get displayedPrice(): number {
    if (!this.cart) return 0;
    
    if (this.useBonus) {
      const discountedPrice = this.cart.totalPrice - this.bonusPoints;
      return Math.max(0, discountedPrice);
    }
    
    return this.cart.totalPrice;
  }

  get discountAmount(): number {
    if (!this.cart || !this.useBonus) return 0;
    return Math.min(this.bonusPoints, this.cart.totalPrice);
  }

  get isEmpty(): boolean {
    return !this.cart || this.cart.items.length === 0;
  }

  get isProcessing(): boolean {
    return this.loading || this.purchasing || this.removingItemId !== null;
  }

  get currentBalance(): number {
    return this.balanceService.getBalance();
  }

  get insufficientFunds(): boolean {
    return this.displayedPrice > 0 && this.currentBalance < this.displayedPrice;
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString();
  }

  formatPrice(price: number): string {
    return `$${price.toFixed(2)}`;
  }

  getItemImage(item: CartItemDto): string {
    if (item.imageUrl) {
      return `url('${item.imageUrl}')`;
    }
    return '';
  }
}
