import { Component, OnInit, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TourService } from '../../../core/services';
import { 
  TourDto, 
  AddKeyPointRequest,
  InterestLabels, 
  DifficultyLabels, 
  TourStatusLabels,
  Interest,
  Difficulty,
  TourStatus
} from '../../../shared/models';
import { LeafletMapComponent, MapCoordinates } from '../../../shared/components';

@Component({
  selector: 'app-manage-tour',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, LeafletMapComponent],
  templateUrl: './manage-tour.component.html',
  styleUrl: './manage-tour.component.css'
})
export class ManageTourComponent implements OnInit {
  @ViewChild(LeafletMapComponent) mapComponent!: LeafletMapComponent;
  
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private tourService = inject(TourService);
  
  tour: TourDto | null = null;
  loading = false;
  error: string | null = null;
  addingKeyPoint = false;
  keyPointError: string | null = null;
  publishing = false;
  publishError: string | null = null;

  keyPointForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    description: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(500)]],
    latitude: [null as number | null, [Validators.required]],
    longitude: [null as number | null, [Validators.required]],
    imageUrl: ['']
  });

  ngOnInit(): void {
    const tourId = this.route.snapshot.paramMap.get('id');
    if (tourId) {
      this.loadTour(tourId);
    } else {
      this.error = 'Tour ID not provided';
    }
  }

  loadTour(tourId: string): void {
    this.loading = true;
    this.error = null;
    
    this.tourService.getTourById(tourId).subscribe({
      next: (response) => {
        this.loading = false;
        if (response.success && response.data) {
          this.tour = response.data;
        } else {
          this.error = response.error?.message || 'Tour not found';
        }
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.error?.message || 'An error occurred while loading the tour';
      }
    });
  }

  onMapClick(coords: MapCoordinates): void {
    this.keyPointForm.patchValue({
      latitude: coords.latitude,
      longitude: coords.longitude
    });
  }

  addKeyPoint(): void {
    if (this.keyPointForm.invalid || !this.tour) {
      this.keyPointForm.markAllAsTouched();
      return;
    }

    this.addingKeyPoint = true;
    this.keyPointError = null;

    const formValue = this.keyPointForm.value;
    const request: AddKeyPointRequest = {
      name: formValue.name,
      description: formValue.description,
      latitude: formValue.latitude,
      longitude: formValue.longitude,
      imageUrl: formValue.imageUrl || ''
    };

    this.tourService.addKeyPoint(this.tour.id, request).subscribe({
      next: (response) => {
        this.addingKeyPoint = false;
        if (response.success && response.data) {
          this.tour!.keyPoints.push(response.data);
          this.keyPointForm.reset();
          if (this.mapComponent) {
            this.mapComponent.clearClickMarker();
          }
        } else {
          this.keyPointError = response.error?.message || 'Failed to add key point';
        }
      },
      error: (err) => {
        this.addingKeyPoint = false;
        this.keyPointError = err.error?.error?.message || 'An error occurred while adding key point';
      }
    });
  }

  publishTour(): void {
    if (!this.tour || !this.canPublish) return;

    this.publishing = true;
    this.publishError = null;

    this.tourService.publishTour(this.tour.id).subscribe({
      next: (response) => {
        this.publishing = false;
        if (response.success && response.data) {
          this.tour = response.data;
        } else {
          this.publishError = response.error?.message || 'Failed to publish tour';
        }
      },
      error: (err) => {
        this.publishing = false;
        this.publishError = err.error?.error?.message || 'An error occurred while publishing';
      }
    });
  }

  get canPublish(): boolean {
    return this.tour !== null && 
           this.tour.status === TourStatus.Draft && 
           this.tour.keyPoints.length >= 2;
  }

  get isDraft(): boolean {
    return this.tour?.status === TourStatus.Draft;
  }

  get sortedKeyPoints() {
    if (!this.tour) return [];
    return [...this.tour.keyPoints].sort((a, b) => a.order - b.order);
  }

  getCategoryLabel(category: Interest): string {
    return InterestLabels[category] || 'Unknown';
  }

  getDifficultyLabel(difficulty: Difficulty): string {
    return DifficultyLabels[difficulty] || 'Unknown';
  }

  getStatusLabel(status: TourStatus): string {
    return TourStatusLabels[status] || 'Unknown';
  }

  getStatusClass(status: TourStatus): string {
    switch (status) {
      case TourStatus.Draft: return 'status-draft';
      case TourStatus.Published: return 'status-published';
      case TourStatus.Canceled: return 'status-canceled';
      default: return '';
    }
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatPrice(price: number): string {
    return `$${price.toFixed(2)}`;
  }
}
