import { Routes } from '@angular/router';
import { HomeComponent } from './layout/home/home.component';
import { LoginComponent } from './layout/login/login.component';
import { ForbiddenComponent } from './layout/forbidden/forbidden.component';
import { NotFoundComponent } from './layout/not-found/not-found.component';
import { TouristAreaComponent } from './features/tourist-area/tourist-area.component';
import { GuideAreaComponent } from './features/guide-area/guide-area.component';
import { AdminAreaComponent } from './features/admin-area/admin-area.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'forbidden', component: ForbiddenComponent },
  
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
