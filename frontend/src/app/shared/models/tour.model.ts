export enum Difficulty {
  Easy = 0,
  Medium = 1,
  Hard = 2
}

export enum Interest {
  Nature = 0,
  Art = 1,
  Sport = 2,
  Shopping = 3,
  Food = 4
}

export enum TourStatus {
  Draft = 0,
  Published = 1,
  Canceled = 2
}

export interface KeyPointDto {
  id: string;
  latitude: number;
  longitude: number;
  name: string;
  description: string;
  imageUrl: string;
  order: number;
}

export interface TourDto {
  id: string;
  guideId: string;
  name: string;
  description: string;
  difficulty: Difficulty;
  category: Interest;
  price: number;
  startDate: string;
  status: TourStatus;
  keyPoints: KeyPointDto[];
}

export interface CreateTourRequest {
  name: string;
  description: string;
  difficulty: Difficulty;
  category: Interest;
  price: number;
  startDate: string;
}

export interface AddKeyPointRequest {
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  imageUrl: string;
}

export const DifficultyLabels: Record<Difficulty, string> = {
  [Difficulty.Easy]: 'Easy',
  [Difficulty.Medium]: 'Medium',
  [Difficulty.Hard]: 'Hard'
};

export const InterestLabels: Record<Interest, string> = {
  [Interest.Nature]: 'Nature',
  [Interest.Art]: 'Art',
  [Interest.Sport]: 'Sport',
  [Interest.Shopping]: 'Shopping',
  [Interest.Food]: 'Food'
};

export const TourStatusLabels: Record<TourStatus, string> = {
  [TourStatus.Draft]: 'Draft',
  [TourStatus.Published]: 'Published',
  [TourStatus.Canceled]: 'Canceled'
};
