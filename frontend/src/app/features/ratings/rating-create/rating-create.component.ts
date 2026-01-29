import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { RatingService } from '../../../core/services/rating.service';
import { CreateRatingRequest } from '../../../shared/models/rating.model';

@Component({
  selector: 'app-rating-create',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './rating-create.component.html',
  styleUrl: './rating-create.component.css'
})
export class RatingCreateComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private ratingService = inject(RatingService);

  tourId: string | null = null;
  loading = false;
  error: string | null = null;
  success: string | null = null;

  ratingForm: FormGroup = this.fb.group({
    score: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    comment: ['']
  }, { validators: this.commentRequiredValidator });

  ngOnInit(): void {
    this.tourId = this.route.snapshot.queryParamMap.get('tourId');
    if (!this.tourId) {
      this.error = 'Tour ID is required';
    }

    // Watch for score changes to update validation
    this.ratingForm.get('score')?.valueChanges.subscribe(() => {
      this.ratingForm.updateValueAndValidity();
    });
  }

  commentRequiredValidator(control: AbstractControl): ValidationErrors | null {
    const score = control.get('score')?.value;
    const comment = control.get('comment')?.value;

    if (score <= 2 && (!comment || comment.trim() === '')) {
      return { commentRequired: true };
    }
    return null;
  }

  get isCommentRequired(): boolean {
    const score = this.ratingForm.get('score')?.value;
    return score <= 2;
  }

  get scoreValue(): number {
    return this.ratingForm.get('score')?.value || 5;
  }

  submitRating(): void {
    if (this.ratingForm.invalid || !this.tourId) {
      this.ratingForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;
    this.success = null;

    const request: CreateRatingRequest = {
      tourId: this.tourId,
      score: this.ratingForm.get('score')?.value,
      comment: this.ratingForm.get('comment')?.value || undefined
    };

    this.ratingService.createRating(request).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success) {
          this.success = 'Rating submitted successfully!';
          setTimeout(() => {
            this.router.navigate(['/tours', this.tourId]);
          }, 1500);
        } else {
          this.error = response.error?.message || 'Failed to submit rating';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while submitting rating';
      }
    });
  }

  getScoreLabel(score: number): string {
    const labels: Record<number, string> = {
      1: 'Very Poor',
      2: 'Poor',
      3: 'Average',
      4: 'Good',
      5: 'Excellent'
    };
    return labels[score] || '';
  }
}
