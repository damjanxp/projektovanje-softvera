import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models';
import { CreateRatingRequest, RatingDto } from '../../shared/models/rating.model';

@Injectable({
  providedIn: 'root'
})
export class RatingService {
  private readonly baseUrl = `${environment.apiUrl}/rating`;
  private http = inject(HttpClient);

  createRating(request: CreateRatingRequest): Observable<ApiResponse<RatingDto>> {
    return this.http.post<ApiResponse<RatingDto>>(this.baseUrl, request);
  }

  getRatingsForTour(tourId: string): Observable<ApiResponse<RatingDto[]>> {
    return this.http.get<ApiResponse<RatingDto[]>>(`${this.baseUrl}/tour/${tourId}`);
  }

  getAverageRatingForTour(tourId: string): Observable<ApiResponse<number | null>> {
    return this.http.get<ApiResponse<number | null>>(`${this.baseUrl}/tour/${tourId}/average`);
  }
}
