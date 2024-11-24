import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Importação dos módulos do Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatNativeDateModule, MatOptionModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { NgChartsModule } from 'ng2-charts';

// Importação dos componentes
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { HomeComponent } from './pages/home/home.component';
import { MenuComponent } from './components/menu/menu.component';
import { FooterComponent } from './components/footer/footer.component';
import { TransacaoTotaisComponent } from './components/transacao-totais/transacao-totais.component';
import { LembretesComponent } from './pages/lembretes/lembretes.component';
import { LembretesDetalhesComponent } from './pages/lembretes/lembretes-detalhes/lembretes-detalhes.component';
import { LembretesExcluirComponent } from './pages/lembretes/lembretes-excluir/lembretes-excluir.component';
import { DashboardComponent } from './pages/financeiro/dashboard/dashboard/dashboard.component';
import { TransacaoListComponent } from './pages/financeiro/transacao/transacao-list/transacao-list.component';
import { TransacaoCreateComponent } from './pages/financeiro/transacao/transacao-create/transacao-create.component';
import { TransacaoEditComponent } from './pages/financeiro/transacao/transacao-edit/transacao-edit.component';
import { TransacaoDetailComponent } from './pages/financeiro/transacao/transacao-detail/transacao-detail.component';
import { TransacaoDeleteComponent } from './pages/financeiro/transacao/transacao-delete/transacao-delete.component';
import { TransacaoFiltroComponent } from './components/transacao-filtro/transacao-filtro.component';
import { TransacaoFiltroModule } from './components/transacao-filtro/transacao-filtro.module';

import { CategoriaListComponent } from './pages/categoria/categoria-list/categoria-list.component';
import { CategoriaCreateComponent } from './pages/categoria/categoria-create/categoria-create.component';
import { CategoriaEditComponent } from './pages/categoria/categoria-edit/categoria-edit.component';
import { CategoriaDetailComponent } from './pages/categoria/categoria-detail/categoria-detail.component';
import { CategoriaDeleteComponent } from './pages/categoria/categoria-delete/categoria-delete.component';

import { AppRoutingModule } from './app-routing.module';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    MenuComponent,
    FooterComponent,
    LembretesComponent,
    LembretesDetalhesComponent,
    LembretesExcluirComponent,
    DashboardComponent,
    TransacaoListComponent,
    TransacaoCreateComponent,
    TransacaoEditComponent,
    TransacaoDetailComponent,
    TransacaoDeleteComponent,
    TransacaoTotaisComponent,
    CategoriaListComponent,
    CategoriaCreateComponent,
    CategoriaEditComponent,
    CategoriaDetailComponent,
    CategoriaDeleteComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    RouterModule,
    NgChartsModule,
    // Material Modules
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatNativeDateModule,
    MatDatepickerModule,
    MatIconModule,
    MatPaginatorModule,
    MatToolbarModule,
    MatMenuModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTableModule,
    MatSortModule,
    MatOptionModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatProgressBarModule,
    MatTooltipModule,
    TransacaoFiltroModule,
    // App Routing
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }