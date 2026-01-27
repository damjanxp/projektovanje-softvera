import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  ApiResponse, 
  TourDto, 
  KeyPointDto, 
  CreateTourRequest, 
  AddKeyPointRequest 
} from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class TourService {
  private readonly baseUrl = `${environment.apiUrl}/tour`;
  private http = inject(HttpClient);

  getPublishedTours(sort: 'asc' | 'desc' = 'asc'): Observable<ApiResponse<TourDto[]>> {
    const params = new HttpParams().set('sort', sort);
    return this.http.get<ApiResponse<TourDto[]>>(`${this.baseUrl}/published`, { params });
  }

  getMyTours(sort: 'asc' | 'desc' = 'asc'): Observable<ApiResponse<TourDto[]>> {
    const params = new HttpParams().set('sort', sort);
    return this.http.get<ApiResponse<TourDto[]>>(`${this.baseUrl}/my`, { params });
  }

  getTourById(tourId: string): Observable<ApiResponse<TourDto>> {
    return this.http.get<ApiResponse<TourDto>>(`${this.baseUrl}/${tourId}`);
  }

  createTour(request: CreateTourRequest): Observable<ApiResponse<TourDto>> {
    return this.http.post<ApiResponse<TourDto>>(this.baseUrl, request);
  }

  addKeyPoint(tourId: string, request: AddKeyPointRequest): Observable<ApiResponse<KeyPointDto>> {
    return this.http.post<ApiResponse<KeyPointDto>>(
      `${this.baseUrl}/${tourId}/key-points`, 
      request
    );
  }

  publishTour(tourId: string): Observable<ApiResponse<TourDto>> {
    return this.http.post<ApiResponse<TourDto>>(
      `${this.baseUrl}/${tourId}/publish`, 
      {}
    );
  }
}
