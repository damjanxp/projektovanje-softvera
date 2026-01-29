import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models';
import { ReplacementTourDto } from '../../shared/models/replacement.model';

@Injectable({
  providedIn: 'root'
})
export class ReplacementService {
  private readonly baseUrl = `${environment.apiUrl}/replacement`;
  private http = inject(HttpClient);

  requestReplacement(tourId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/request/${tourId}`, {});
  }

  getReplacementTours(): Observable<ApiResponse<ReplacementTourDto[]>> {
    return this.http.get<ApiResponse<ReplacementTourDto[]>>(this.baseUrl);
  }

  takeOverTour(tourId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/take/${tourId}`, {});
  }
}
