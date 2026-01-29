import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PurchaseRequest, PurchaseResponse, BonusPointsDto } from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class PurchaseService {
  private readonly baseUrl = `${environment.apiUrl}`;
  private http = inject(HttpClient);

  confirmPurchase(useBonus: boolean): Observable<ApiResponse<PurchaseResponse>> {
    return this.http.post<ApiResponse<PurchaseResponse>>(
      `${this.baseUrl}/purchase/confirm?useBonus=${useBonus}`, 
      {}
    );
  }

  getBonusPoints(): Observable<ApiResponse<BonusPointsDto>> {
    return this.http.get<ApiResponse<BonusPointsDto>>(`${this.baseUrl}/users/me/bonus-points`);
  }
}
