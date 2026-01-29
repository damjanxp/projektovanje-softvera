import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models';

export interface BlockedUserDto {
  username: string;
  role: string;
  blockCount: number;
  blockedAt: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly baseUrl = `${environment.apiUrl}/admin`;
  private http = inject(HttpClient);

  getBlockedUsers(): Observable<ApiResponse<BlockedUserDto[]>> {
    return this.http.get<ApiResponse<BlockedUserDto[]>>(`${this.baseUrl}/blocked-users`);
  }

  unblockUser(username: string): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.baseUrl}/unblock/${username}`, {});
  }
}
