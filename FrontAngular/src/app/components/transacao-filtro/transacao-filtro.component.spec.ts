import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

export interface FiltroData {
  tipoFiltro: 'mes' | 'periodo';
  mesAno?: Date;
  dataInicio?: Date;
  dataFim?: Date;
}

@Component({
  selector: 'app-transacao-filtro',
  templateUrl: './transacao-filtro.component.html',
  styleUrls: ['./transacao-filtro.component.scss']
})
export class TransacaoFiltroComponent implements OnInit, OnDestroy {
  @Output() filtroChange = new EventEmitter<FiltroData>();
  
  filterForm!: FormGroup;
  maxDate: Date = new Date();
  private destroy$ = new Subject<void>();

  constructor(private fb: FormBuilder) {
    this.initializeForm();
  }

  ngOnInit(): void {
    this.setupFormSubscription();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.filterForm = this.fb.group({
      tipoFiltro: ['mes'],
      mesAno: [new Date()],
      dataInicio: [null],
      dataFim: [null]
    });
  }

  private setupFormSubscription(): void {
    this.filterForm.get('tipoFiltro')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.filterForm.patchValue({
          mesAno: new Date(),
          dataInicio: null,
          dataFim: null
        });
        this.aplicarFiltro();
      });
  }

  setMonthAndYear(date: Date, datepicker: any): void {
    const ctrlValue = this.filterForm.get('mesAno')?.value;
    const ctrlDate = new Date(ctrlValue);
    ctrlDate.setMonth(date.getMonth());
    ctrlDate.setFullYear(date.getFullYear());
    this.filterForm.get('mesAno')?.setValue(ctrlDate);
    datepicker.close();
    this.aplicarFiltro();
  }

  // Método tornado público
  isPeriodoValido(): boolean {
    const inicio = this.filterForm.get('dataInicio')?.value;
    const fim = this.filterForm.get('dataFim')?.value;
    return inicio && fim && inicio <= fim;
  }

  aplicarFiltro(): void {
    if (!this.filterForm.valid) return;
    
    // Garantir que as datas do período estejam completas
    if (this.filterForm.value.tipoFiltro === 'periodo') {
      if (!this.isPeriodoValido()) return;
    }
    
    this.filtroChange.emit(this.filterForm.value);
  }

  limparFiltro(): void {
    this.filterForm.patchValue({
      tipoFiltro: 'mes',
      mesAno: new Date(),
      dataInicio: null,
      dataFim: null
    });
    this.aplicarFiltro();
  }
}