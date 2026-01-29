export interface ReplacementTourDto {
  id: string;
  currentGuideId: string;
  name: string;
  description: string;
  startDate: string;
  price: number;
  category: number;
  difficulty: number;
  replacementRequestedAt: string;
}

export interface TourWithReplacementDto {
  id: string;
  guideId: string;
  name: string;
  description: string;
  difficulty: number;
  category: number;
  price: number;
  startDate: string;
  status: number;
  needsReplacement: boolean;
  replacementRequestedAt?: string;
}
