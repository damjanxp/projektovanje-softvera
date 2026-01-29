export interface CartItemDto {
  tourId: string;
  name: string;
  price: number;
  startDate: string;
}

export interface CartDto {
  items: CartItemDto[];
  totalPrice: number;
}

export interface PurchaseRequest {
  useBonus: boolean;
}

export interface PurchaseResponse {
  finalPrice: number;
  usedBonusPoints: number;
}

export interface BonusPointsDto {
  points: number;
}
