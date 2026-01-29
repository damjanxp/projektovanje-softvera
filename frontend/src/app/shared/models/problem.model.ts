export enum ProblemStatus {
  Pending = 0,
  Resolved = 1,
  InReview = 2,
  Rejected = 3
}

export const ProblemStatusLabels: Record<ProblemStatus, string> = {
  [ProblemStatus.Pending]: 'Pending',
  [ProblemStatus.Resolved]: 'Resolved',
  [ProblemStatus.InReview]: 'In Review',
  [ProblemStatus.Rejected]: 'Rejected'
};

export interface CreateProblemRequest {
  tourId: string;
  title: string;
  description: string;
}

export interface ProblemDto {
  id: string;
  touristId: string;
  tourId: string;
  title: string;
  description: string;
  status: ProblemStatus;
  createdAt: string;
}

export interface ProblemEventDto {
  id: string;
  problemId: string;
  oldStatus: ProblemStatus;
  newStatus: ProblemStatus;
  changedAt: string;
  changedByRole: string;
  changedByUserId: string;
}
