import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProblemService } from '../../../core/services/problem.service';
import { CreateProblemRequest } from '../../../shared/models/problem.model';

@Component({
  selector: 'app-problem-create',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './problem-create.component.html',
  styleUrl: './problem-create.component.css'
})
export class ProblemCreateComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private problemService = inject(ProblemService);

  tourId: string | null = null;
  loading = false;
  error: string | null = null;

  problemForm: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.minLength(20), Validators.maxLength(2000)]]
  });

  ngOnInit(): void {
    this.tourId = this.route.snapshot.queryParamMap.get('tourId');
    if (!this.tourId) {
      this.error = 'Tour ID is required';
    }
  }

  submitProblem(): void {
    if (this.problemForm.invalid || !this.tourId) {
      this.problemForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    const request: CreateProblemRequest = {
      tourId: this.tourId,
      title: this.problemForm.get('title')?.value,
      description: this.problemForm.get('description')?.value
    };

    this.problemService.createProblem(request).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.router.navigate(['/problems', response.data.id]);
        } else {
          this.error = response.error?.message || 'Failed to submit problem';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while submitting the problem';
      }
    });
  }
}
