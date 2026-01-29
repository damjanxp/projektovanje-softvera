import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, CartDto, CartItemDto } from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private readonly baseUrl = `${environment.apiUrl}/cart`;
  private http = inject(HttpClient);
  
  private cartItemsSubject = new BehaviorSubject<Set<string>>(new Set());
  cartItems$ = this.cartItemsSubject.asObservable();

  addToCart(tourId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/add`, { tourId }).pipe(
      tap((response: ApiResponse<void>) => {
        if (response.success) {
          const currentItems = this.cartItemsSubject.value;
          currentItems.add(tourId);
          this.cartItemsSubject.next(new Set(currentItems));
        }
      })
    );
  }

  getCart(): Observable<ApiResponse<CartDto>> {
    return this.http.get<ApiResponse<CartDto>>(this.baseUrl).pipe(
      tap((response: ApiResponse<CartDto>) => {
        if (response.success && response.data) {
          const tourIds = new Set<string>(response.data.items.map((item: CartItemDto) => item.tourId));
          this.cartItemsSubject.next(tourIds);
        }
      })
    );
  }

  removeFromCart(tourId: string): Observable<ApiResponse<void>> {
    return this.http.post<ApiResponse<void>>(`${this.baseUrl}/remove`, { tourId }).pipe(
      tap((response: ApiResponse<void>) => {
        if (response.success) {
          const currentItems = this.cartItemsSubject.value;
          currentItems.delete(tourId);
          this.cartItemsSubject.next(new Set(currentItems));
        }
      })
    );
  }

  isInCart(tourId: string): Observable<boolean> {
    return this.cartItems$.pipe(
      map(items => items.has(tourId))
    );
  }

  isInCartSync(tourId: string): boolean {
    return this.cartItemsSubject.value.has(tourId);
  }

  clearLocalCart(): void {
    this.cartItemsSubject.next(new Set());
  }
}
