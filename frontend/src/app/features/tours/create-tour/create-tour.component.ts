import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TourService } from '../../../core/services';
import { 
  CreateTourRequest, 
  Difficulty, 
  Interest, 
  DifficultyLabels, 
  InterestLabels 
} from '../../../shared/models';

@Component({
  selector: 'app-create-tour',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './create-tour.component.html',
  styleUrl: './create-tour.component.css'
})
export class CreateTourComponent {
  private fb = inject(FormBuilder);
  private tourService = inject(TourService);
  private router = inject(Router);
  
  loading = false;
  error: string | null = null;

  tourForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]],
    category: [Interest.Nature, [Validators.required]],
    difficulty: [Difficulty.Easy, [Validators.required]],
    price: [0, [Validators.required, Validators.min(0)]],
    startDate: ['', [Validators.required]]
  });

  difficulties = [
    { value: Difficulty.Easy, label: DifficultyLabels[Difficulty.Easy] },
    { value: Difficulty.Medium, label: DifficultyLabels[Difficulty.Medium] },
    { value: Difficulty.Hard, label: DifficultyLabels[Difficulty.Hard] }
  ];

  categories = [
    { value: Interest.Nature, label: InterestLabels[Interest.Nature] },
    { value: Interest.Art, label: InterestLabels[Interest.Art] },
    { value: Interest.Sport, label: InterestLabels[Interest.Sport] },
    { value: Interest.Shopping, label: InterestLabels[Interest.Shopping] },
    { value: Interest.Food, label: InterestLabels[Interest.Food] }
  ];

  onSubmit(): void {
    if (this.tourForm.invalid) {
      this.tourForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = null;

    const formValue = this.tourForm.value;
    const request: CreateTourRequest = {
      name: formValue.name,
      description: formValue.description,
      category: Number(formValue.category),
      difficulty: Number(formValue.difficulty),
      price: Number(formValue.price),
      startDate: new Date(formValue.startDate).toISOString()
    };

    this.tourService.createTour(request).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.router.navigate(['/guide/tours', response.data.id]);
        } else {
          this.error = response.error?.message || 'Failed to create tour';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while creating the tour';
      }
    });
  }

  getMinDate(): string {
    return new Date().toISOString().split('T')[0];
  }
}
