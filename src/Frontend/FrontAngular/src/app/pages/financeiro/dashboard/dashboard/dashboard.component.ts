import { Component, OnInit, ViewChild, AfterViewInit, OnDestroy } from '@angular/core';
import { TransacaoService } from '@/app/services/transacao.service';
import { Transacao } from '@/app/models/transacao.model';
import { Categoria } from '@/app/models/categoria.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { ResumoFinanceiro } from '@/app/models/resumo-financeiro.model';
import { FiltroData } from '@/app/models/filtro-data.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  transacoes: Transacao[] = [];
  transacoesFiltradas: Transacao[] = [];
  isLoading = false;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  // Dados da Tabela
  dataSource: MatTableDataSource<Transacao> = new MatTableDataSource<Transacao>();
  displayedColumns: string[] = ['data', 'descricao', 'tipo', 'valor'];

  // Dados do Resumo
  resumoFinanceiro: ResumoFinanceiro = {
    totalEntradas: 0,
    totalSaidas: 0,
    saldo: 0,
    mediaMensal: 0
  };

  // Dados dos Gráficos
  barChartData: any = {};
  lineChartData: any = {};
  pieChartData: any = {};
  
  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { 
        ticks: { autoSkip: true, maxTicksLimit: 20 }
      }
    },
    plugins: {
      legend: {
        position: 'top',
        labels: { font: { size: 12 }, padding: 10 }
      },
      tooltip: {
        callbacks: {
          label: (tooltipItem: any) => `R$ ${tooltipItem.raw.toFixed(2)}`
        }
      }
    }
  };

  constructor(private transacaoService: TransacaoService) {}

  ngOnInit(): void {
    this.loadData();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadData(): void {
    this.isLoading = true;
    this.error = null;
  
    this.transacaoService.getTransacoes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          console.log('Dados recebidos:', data);
          this.transacoes = data;
          
          this.aplicarFiltroMesAtual();
          
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao carregar transações:', error);
          this.error = 'Erro ao carregar as transações.';
          this.isLoading = false;
        }
      });
  }

  onFiltroAplicado(filtro: FiltroData): void {
    let transacoesFiltradas = [...this.transacoes];

    if (filtro.tipoFiltro === 'mes' && filtro.mesAno) {
      const mesAnoSelecionado = new Date(filtro.mesAno);
      transacoesFiltradas = this.transacoes.filter(transacao => {
        const dataTransacao = new Date(transacao.data);
        return dataTransacao.getMonth() === mesAnoSelecionado.getMonth() &&
               dataTransacao.getFullYear() === mesAnoSelecionado.getFullYear();
      });
    } else if (filtro.tipoFiltro === 'periodo' && filtro.dataInicio && filtro.dataFim) {
      const inicio = new Date(filtro.dataInicio);
      const fim = new Date(filtro.dataFim);
      fim.setHours(23, 59, 59);

      transacoesFiltradas = this.transacoes.filter(transacao => {
        const dataTransacao = new Date(transacao.data);
        return dataTransacao >= inicio && dataTransacao <= fim;
      });
    }

    this.transacoesFiltradas = transacoesFiltradas;
    this.dataSource.data = transacoesFiltradas;
    this.atualizarResumoFinanceiro(transacoesFiltradas);
    this.setupCharts();
  }

  aplicarFiltroTabela(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  aplicarFiltroMesAtual(): void {
    const hoje = new Date();
    const mesAtual = hoje.getMonth();
    const anoAtual = hoje.getFullYear();
    
    const transacoesFiltradas = this.transacoes.filter(transacao => {
      const dataTransacao = new Date(transacao.data);
      return dataTransacao.getMonth() === mesAtual && 
             dataTransacao.getFullYear() === anoAtual;
    });
    
    this.transacoesFiltradas = transacoesFiltradas;
    this.dataSource.data = transacoesFiltradas;
    this.atualizarResumoFinanceiro(transacoesFiltradas);
    this.setupCharts();
  }
  

  reloadData(): void {
    this.loadData();
  }

  private atualizarResumoFinanceiro(transacoes: Transacao[]): void {
    const totalEntradas = transacoes
      .filter(t => t.tipo === 'Entrada')
      .reduce((sum, t) => sum + t.valor, 0);

    const totalSaidas = transacoes
      .filter(t => t.tipo === 'Saida')
      .reduce((sum, t) => sum + t.valor, 0);

    this.resumoFinanceiro = {
      totalEntradas: totalEntradas,
      totalSaidas: totalSaidas,
      saldo: totalEntradas - totalSaidas,
      mediaMensal: this.calcularMediaMensal(transacoes)
    };
  }

  private calcularMediaMensal(transacoes: Transacao[]): number {
    if (transacoes.length === 0) return 0;

    const datas = transacoes.map(t => new Date(t.data));
    const minData = new Date(Math.min(...datas.map(d => d.getTime())));
    const maxData = new Date(Math.max(...datas.map(d => d.getTime())));
    
    const mesesDiferenca = (maxData.getFullYear() - minData.getFullYear()) * 12 + 
      (maxData.getMonth() - minData.getMonth()) + 1;

    return (this.resumoFinanceiro.saldo) / Math.max(1, mesesDiferenca);
  }

  private setupCharts(): void {
    const dadosFiltrados = this.transacoesFiltradas;
    
    // Configuração do gráfico de barras
    const entradas = dadosFiltrados
      .filter(t => t.tipo === 'Entrada')
      .reduce((sum, t) => sum + t.valor, 0);
    
    const saidas = dadosFiltrados
      .filter(t => t.tipo === 'Saida')
      .reduce((sum, t) => sum + t.valor, 0);

    this.barChartData = {
      labels: ['Atual'],
      datasets: [
        {
          label: 'Entradas',
          data: [entradas],
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          borderColor: 'rgba(75, 192, 192, 1)',
          borderWidth: 1
        },
        {
          label: 'Saídas',
          data: [saidas],
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        }
      ]
    };

    // Configuração do gráfico de linha (evolução do saldo)
    const saldoDiario = this.calcularSaldoDiario(dadosFiltrados);
    this.lineChartData = {
      labels: saldoDiario.map(item => item.data),
      datasets: [{
        label: 'Saldo',
        data: saldoDiario.map(item => item.saldo),
        fill: false,
        borderColor: 'rgb(75, 192, 192)',
        tension: 0.1
      }]
    };

    // Configuração do gráfico de pizza (despesas por categoria)
    const despesasPorCategoria = this.calcularDespesasPorCategoria(dadosFiltrados);
    this.pieChartData = {
      labels: Object.keys(despesasPorCategoria),
      datasets: [{
        data: Object.values(despesasPorCategoria),
        backgroundColor: [
          'rgba(255, 99, 132, 0.8)',
          'rgba(54, 162, 235, 0.8)',
          'rgba(255, 206, 86, 0.8)',
          'rgba(75, 192, 192, 0.8)'
        ]
      }]
    };
  }

  private calcularSaldoDiario(transacoes: Transacao[]): any[] {
    return transacoes
      .sort((a, b) => new Date(a.data).getTime() - new Date(b.data).getTime())
      .reduce((acc: any[], transaction) => {
        const lastBalance = acc.length > 0 ? acc[acc.length - 1].saldo : 0;
        const newBalance = transaction.tipo === 'Entrada' 
          ? lastBalance + transaction.valor 
          : lastBalance - transaction.valor;
        
        acc.push({
          data: new Date(transaction.data).toLocaleDateString(),
          saldo: newBalance
        });
        return acc;
      }, []);
  }

  private calcularDespesasPorCategoria(transacoes: Transacao[]): Record<string, number> {
    return transacoes
      .filter(t => t.tipo === 'Saida')
      .reduce((acc: Record<string, number>, t) => {
        const categoria = t.categoria.nome;
        acc[categoria] = (acc[categoria] || 0) + t.valor;
        return acc;
      }, {});
  }
}