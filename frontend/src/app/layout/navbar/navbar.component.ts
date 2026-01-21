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

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  get username(): string | null {
    return this.authService.getUsername();
  }

  get userRole(): UserRole | null {
    return this.authService.getUserRole();
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

  logout(): void {
    this.authService.logout();
  }
}
