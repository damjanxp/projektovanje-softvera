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
import { CartComponent } from './features/cart/cart.component';
import { ReplacementBoardComponent } from './features/replacements/replacement-board/replacement-board.component';
import { RatingCreateComponent } from './features/ratings/rating-create/rating-create.component';
import { ProblemCreateComponent } from './features/problems/problem-create/problem-create.component';
import { ProblemDetailsComponent } from './features/problems/problem-details/problem-details.component';
import { PurchasedToursListComponent } from './features/tours/purchased-tours-list/purchased-tours-list.component';
import { BlockedUsersComponent } from './features/admin/blocked-users/blocked-users.component';
import { ProblemsReviewComponent } from './features/admin/problems-review/problems-review.component';
import { GuideProblemsListComponent } from './features/guide/problems-list/problems-list.component';
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
  { 
    path: 'cart', 
    component: CartComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Tourist'] }
  },
  { 
    path: 'tourist/my-purchases', 
    component: PurchasedToursListComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Tourist'] }
  },
  { 
    path: 'tourist/ratings/new', 
    component: RatingCreateComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Tourist'] }
  },
  { 
    path: 'tourist/problems/new', 
    component: ProblemCreateComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Tourist'] }
  },
  
  // Problem details - accessible by all authenticated users
  { 
    path: 'problems/:id', 
    component: ProblemDetailsComponent,
    canActivate: [authGuard]
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
  { 
    path: 'guide/problems', 
    component: GuideProblemsListComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['Guide'] }
  },
  { 
    path: 'guide/replacements', 
    component: ReplacementBoardComponent,
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
    { 
      path: 'admin/blocked-users', 
      component: BlockedUsersComponent,
      canActivate: [authGuard, roleGuard],
      data: { roles: ['Admin'] }
    },
    { 
      path: 'admin/problems-review', 
      component: ProblemsReviewComponent,
      canActivate: [authGuard, roleGuard],
      data: { roles: ['Admin'] }
    },
  
    // 404 - must be last
    { path: '**', component: NotFoundComponent }
  ];
