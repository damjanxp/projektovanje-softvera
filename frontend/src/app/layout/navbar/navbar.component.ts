import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService, UserRole } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  private authService = inject(AuthService);

  // Icon Paths
  readonly PNG_LOGO_PATH = 'assets/logo.png';
  readonly PNG_TOURS_PATH = 'assets/icons/tours.png';
  readonly PNG_CART_PATH = 'assets/icons/cart.png';
  readonly PNG_MYAREA_PATH = 'assets/icons/myarea.png';
  readonly PNG_LOGOUT_PATH = 'assets/icons/logout.png';
  readonly PNG_LOGIN_PATH = 'assets/icons/login.png';
  
  readonly APP_NAME = 'Explore with Soul';

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  get username(): string | null {
    return this.authService.getUsername();
  }

  get userRole(): UserRole | null {
    return this.authService.getUserRole();
  }

  get isTourist(): boolean {
    return this.userRole === 'Tourist';
  }

  get isGuide(): boolean {
    return this.userRole === 'Guide';
  }

  get roleAreaLink(): string {
    const role = this.userRole;
    switch (role) {
      case 'Tourist': return '/tourist-area';
      case 'Guide': return '/guide-area';
      case 'Admin': return '/admin-area';
      default: return '/';
    }
  }

  get roleAreaLabel(): string {
    const role = this.userRole;
    switch (role) {
      case 'Tourist': return 'My Area';
      case 'Guide': return 'Guide Area';
      case 'Admin': return 'Admin';
      default: return 'My Area';
    }
  }

  logout(): void {
    this.authService.logout();
  }
}
