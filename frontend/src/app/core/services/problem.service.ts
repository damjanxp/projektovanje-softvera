import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models';
import { CreateProblemRequest, ProblemDto, ProblemEventDto } from '../../shared/models/problem.model';

@Injectable({
  providedIn: 'root'
})
export class ProblemService {
  private readonly baseUrl = `${environment.apiUrl}/problem`;
  private http = inject(HttpClient);

  createProblem(request: CreateProblemRequest): Observable<ApiResponse<ProblemDto>> {
    return this.http.post<ApiResponse<ProblemDto>>(this.baseUrl, request);
  }

  getProblem(problemId: string): Observable<ApiResponse<ProblemDto>> {
    return this.http.get<ApiResponse<ProblemDto>>(`${this.baseUrl}/${problemId}`);
  }

  getProblemEvents(problemId: string): Observable<ApiResponse<ProblemEventDto[]>> {
    return this.http.get<ApiResponse<ProblemEventDto[]>>(`${this.baseUrl}/${problemId}/events`);
  }

  getProblemsForTour(tourId: string): Observable<ApiResponse<ProblemDto[]>> {
    return this.http.get<ApiResponse<ProblemDto[]>>(`${this.baseUrl}/tour/${tourId}`);
  }

  getProblemsInReview(): Observable<ApiResponse<ProblemDto[]>> {
    return this.http.get<ApiResponse<ProblemDto[]>>(`${this.baseUrl}/in-review`);
  }

  resolveProblem(problemId: string): Observable<ApiResponse<ProblemDto>> {
    return this.http.post<ApiResponse<ProblemDto>>(`${this.baseUrl}/${problemId}/resolve`, {});
  }

  sendToReview(problemId: string): Observable<ApiResponse<ProblemDto>> {
    return this.http.post<ApiResponse<ProblemDto>>(`${this.baseUrl}/${problemId}/review`, {});
  }

  rejectProblem(problemId: string): Observable<ApiResponse<ProblemDto>> {
    return this.http.post<ApiResponse<ProblemDto>>(`${this.baseUrl}/${problemId}/reject`, {});
  }

  reopenProblem(problemId: string): Observable<ApiResponse<ProblemDto>> {
    return this.http.post<ApiResponse<ProblemDto>>(`${this.baseUrl}/${problemId}/reopen`, {});
  }
}
