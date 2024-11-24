import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Transacao } from '@/app/models/transacao.model';
import { ResumoFinanceiro } from '@/app/models/resumo-financeiro.model';

@Component({
  selector: 'app-transacao-totais',
  templateUrl: './transacao-totais.component.html',
  styleUrls: ['./transacao-totais.component.scss']
})
export class TransacaoTotaisComponent implements OnChanges {
  @Input() transacoes: Transacao[] = [];
  @Input() resumo?: ResumoFinanceiro;

  cardData = [
    { 
      label: 'Total Entradas', 
      value: () => this.resumo?.totalEntradas || 0,
      icon: 'trending_up',
      class: 'income',
      colorClass: 'positive'
    },
    { 
      label: 'Total Saídas', 
      value: () => this.resumo?.totalSaidas || 0,
      icon: 'trending_down',
      class: 'expense',
      colorClass: 'negative'
    },
    { 
      label: 'Saldo', 
      value: () => this.resumo?.saldo || 0,
      icon: 'account_balance_wallet',
      class: 'balance',
      colorClass: (value: number) => value > 0 ? 'positive' : value < 0 ? 'negative' : ''
    },
    { 
      label: 'Média Mensal', 
      value: () => this.resumo?.mediaMensal || 0,
      icon: 'show_chart',
      class: 'average',
      colorClass: (value: number) => value > 0 ? 'positive' : value < 0 ? 'negative' : ''
    }
  ];

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumo && changes['transacoes']?.currentValue) {
      this.calcularResumo();
    }
  }

  private calcularResumo(): void {
    const totalEntradas = this.calcularTotal('Entrada');
    const totalSaidas = this.calcularTotal('Saida');
    const saldo = totalEntradas - totalSaidas;
    
    this.resumo = {
      totalEntradas,
      totalSaidas,
      saldo,
      mediaMensal: this.calcularMediaMensal(saldo)
    };
  }

  private calcularTotal(tipo: 'Entrada' | 'Saida'): number {
    return this.transacoes
      .filter(t => t.tipo === tipo)
      .reduce((sum, t) => sum + t.valor, 0);
  }

  private calcularMediaMensal(saldo: number): number {
    return this.transacoes.length > 0 ? saldo / 12 : 0;
  }

  getColorClass(card: any, value: number): string {
    return typeof card.colorClass === 'function' ? card.colorClass(value) : card.colorClass;
  }
}