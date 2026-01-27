import { Routes } from '@angular/router';
import { HomeComponent } from './layout/home/home.component';
import { LoginComponent } from './layout/login/login.component';
import { ForbiddenComponent } from './layout/forbidden/forbidden.component';
import { NotFoundComponent } from './layout/not-found/not-found.component';
import { TouristAreaComponent } from './features/tourist-area/tourist-area.component';
import { GuideAreaComponent } from './features/guide-area/guide-area.component';
import { AdminAreaComponent } from './features/admin-area/admin-area.component';
import { PublicToursListComponent } from './features/tours/public-tours-list/public-tours-list.component';
import { TourDetailsComponent } from './features/tours/tour-details/tour-details.component';
import { MyToursComponent } from './features/tours/my-tours/my-tours.component';
import { CreateTourComponent } from './features/tours/create-tour/create-tour.component';
import { ManageTourComponent } from './features/tours/manage-tour/manage-tour.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forbidden', component: ForbiddenComponent },
  
  // Public tour routes (no auth required)
  { path: 'tours', component: PublicToursListComponent },
  { path: 'tours/:id', component: TourDetailsComponent },
  
  // Protected routes - Tourist only
  { 
    path: 'tourist-area', 
    component: TouristAreaComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Tourist'] }
  },
  
  // Protected routes - Guide only
  { 
    path: 'guide-area', 
    component: GuideAreaComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Guide'] }
  },
  { 
    path: 'guide/tours', 
    component: MyToursComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Guide'] }
  },
  { 
    path: 'guide/tours/create', 
    component: CreateTourComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Guide'] }
  },
  { 
    path: 'guide/tours/:id', 
    component: ManageTourComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Guide'] }
  },
  
  // Protected routes - Admin only
  { 
    path: 'admin-area', 
    component: AdminAreaComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Admin'] }
  },
  
  // 404 - must be last
  { path: '**', component: NotFoundComponent }
];
