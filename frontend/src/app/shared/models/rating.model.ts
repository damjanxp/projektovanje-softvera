export interface CreateRatingRequest {
  tourId: string;
  score: number;
  comment?: string;
}

export interface RatingDto {
  id: string;
  touristId: string;
  tourId: string;
  score: number;
  comment?: string;
  createdAt: string;
}
