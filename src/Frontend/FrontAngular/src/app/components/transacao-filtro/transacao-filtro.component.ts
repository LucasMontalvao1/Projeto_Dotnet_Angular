import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatDatepicker } from '@angular/material/datepicker';

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
  @Output() filtroAplicado = new EventEmitter<FiltroData>();
  @Output() recarregar = new EventEmitter<void>();
  
  filterForm!: FormGroup;
  maxDate: Date = new Date();
  private destroy$ = new Subject<void>();

  constructor(private fb: FormBuilder) {}

  ngOnInit(): void {
    this.initializeForm();
    this.setupFormSubscription();

    setTimeout(() => {
      this.aplicarFiltro();
    }, 0);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.filterForm = this.fb.group({
      tipoFiltro: ['mes'],
      mesAno: [new Date(), Validators.required],
      dataInicio: [null],
      dataFim: [null]
    });

    // Aplicar validadores condicionalmente
    this.filterForm.get('tipoFiltro')?.valueChanges.subscribe(tipo => {
      const dataInicioControl = this.filterForm.get('dataInicio');
      const dataFimControl = this.filterForm.get('dataFim');
      const mesAnoControl = this.filterForm.get('mesAno');

      if (tipo === 'periodo') {
        dataInicioControl?.setValidators([Validators.required]);
        dataFimControl?.setValidators([Validators.required]);
        mesAnoControl?.clearValidators();
      } else {
        dataInicioControl?.clearValidators();
        dataFimControl?.clearValidators();
        mesAnoControl?.setValidators([Validators.required]);
      }

      dataInicioControl?.updateValueAndValidity();
      dataFimControl?.updateValueAndValidity();
      mesAnoControl?.updateValueAndValidity();
    });
  }

  private setupFormSubscription(): void {
    this.filterForm.get('tipoFiltro')?.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.resetarFiltros();
      });
  }

  private resetarFiltros(): void {
    const tipoFiltro = this.filterForm.get('tipoFiltro')?.value;
    
    this.filterForm.patchValue({
      mesAno: tipoFiltro === 'mes' ? new Date() : null,
      dataInicio: null,
      dataFim: null
    });
  }

  setMonthAndYear(date: Date, datepicker: MatDatepicker<any>): void {
    const ctrlValue = this.filterForm.get('mesAno')?.value;
    const ctrlDate = new Date(ctrlValue || date);
    ctrlDate.setMonth(date.getMonth());
    ctrlDate.setFullYear(date.getFullYear());
    this.filterForm.get('mesAno')?.setValue(ctrlDate);
    datepicker.close();
    this.aplicarFiltro();
  }

  aplicarFiltro(): void {
    if (this.filterForm.invalid) return;
    
    const formValue = this.filterForm.value;
    const filtro: FiltroData = {
      tipoFiltro: formValue.tipoFiltro,
      mesAno: formValue.tipoFiltro === 'mes' ? formValue.mesAno : undefined,
      dataInicio: formValue.tipoFiltro === 'periodo' ? formValue.dataInicio : undefined,
      dataFim: formValue.tipoFiltro === 'periodo' ? formValue.dataFim : undefined
    };

    this.filtroAplicado.emit(filtro);
  }

  limparFiltro(): void {
    this.filterForm.reset({
      tipoFiltro: 'mes',
      mesAno: new Date(),
      dataInicio: null,
      dataFim: null
    });
    this.aplicarFiltro();
    this.recarregar.emit();
  }

  isPeriodoValido(): boolean {
    if (this.filterForm.get('tipoFiltro')?.value !== 'periodo') return true;
    
    const inicio = this.filterForm.get('dataInicio')?.value;
    const fim = this.filterForm.get('dataFim')?.value;
    
    return inicio && fim && inicio <= fim;
  }

  formatarData(data: Date): string {
    if (!data) return '';
    return new Date(data).toLocaleDateString('pt-BR');
  }

  getMesAtual(): string {
    const mesAno = this.filterForm.get('mesAno')?.value;
    if (!mesAno) return '';
    
    return new Date(mesAno).toLocaleDateString('pt-BR', { 
      month: 'long',
      year: 'numeric'
    });
  }
}