import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AdminService } from '../../../core/services/admin.service';

export interface BlockedUserDto {
  username: string;
  role: string;
  blockCount: number;
  blockedAt: string | null;
}

@Component({
  selector: 'app-blocked-users',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './blocked-users.component.html',
  styleUrl: './blocked-users.component.css'
})
export class BlockedUsersComponent implements OnInit {
  private adminService = inject(AdminService);

  blockedUsers: BlockedUserDto[] = [];
  loading = false;
  error: string | null = null;
  unblocking: { [username: string]: boolean } = {};
  successMessage: string | null = null;

  ngOnInit(): void {
    this.loadBlockedUsers();
  }

  loadBlockedUsers(): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;

    this.adminService.getBlockedUsers().subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.blockedUsers = response.data;
        } else {
          this.error = response.error?.message || 'Failed to load blocked users';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading blocked users';
      }
    });
  }

  unblockUser(username: string): void {
    if (this.unblocking[username]) return;

    this.unblocking[username] = true;
    this.error = null;
    this.successMessage = null;

    this.adminService.unblockUser(username).subscribe({
      next: (response) => {
        this.unblocking[username] = false;
        if (response.success) {
          this.successMessage = response.data || `User ${username} has been unblocked`;
          this.loadBlockedUsers(); // Reload the list
        } else {
          this.error = response.error?.message || 'Failed to unblock user';
        }
      },
      error: (err) => {
        this.unblocking[username] = false;
        this.error = err.error?.error?.message || 'An error occurred while unblocking the user';
      }
    });
  }

  formatDate(dateString: string | null): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getRoleBadgeClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'tourist': return 'role-tourist';
      case 'guide': return 'role-guide';
      case 'admin': return 'role-admin';
      default: return 'role-unknown';
    }
  }
}
