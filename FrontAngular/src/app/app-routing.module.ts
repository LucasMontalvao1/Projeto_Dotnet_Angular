import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { DashboardComponent } from './pages/financeiro/dashboard/dashboard/dashboard.component';
import { TransacaoListComponent } from './pages/financeiro/transacao/transacao-list/transacao-list.component';
import { CategoriaListComponent } from './pages/categoria/categoria-list/categoria-list.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'transacao', component: TransacaoListComponent, canActivate: [AuthGuard] },
  { path: 'categoria', component: CategoriaListComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }