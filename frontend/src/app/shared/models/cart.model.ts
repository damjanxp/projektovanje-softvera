export interface CartItemDto {
  tourId: string;
  name: string;
  price: number;
  startDate: string;
  imageUrl?: string;
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

export interface PurchasedTourDto {
  tourId: string;
  tourName: string;
  priceAtPurchase: number;
}

export interface PurchaseResultDto {
  purchaseId: string;
  originalPrice: number;
  finalPrice: number;
  bonusPointsUsed: number;
  bonusPointsEarned: number;
  totalBonusPoints: number;
  purchasedAt: string;
  tours: PurchasedTourDto[];
}

export interface BonusPointsDto {
  points: number;
}
