import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

// Material Imports
import { MatCardModule } from '@angular/material/card';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

// Component Import
import { TransacaoFiltroComponent } from './transacao-filtro.component';

@NgModule({
  declarations: [
    TransacaoFiltroComponent  
  ],
  imports: [
    // Módulos Angular
    CommonModule,
    ReactiveFormsModule,
    
    // Módulos Material
    MatCardModule,
    MatButtonToggleModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule
  ],
  exports: [
    TransacaoFiltroComponent  
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA] 
})
export class TransacaoFiltroModule { }