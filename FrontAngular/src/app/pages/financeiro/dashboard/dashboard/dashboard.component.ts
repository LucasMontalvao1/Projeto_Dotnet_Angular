import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { TransacaoService } from '@/app/services/transacao.service';
import { Transacao } from '@/app/models/transacao.model';
import { Categoria } from '@/app/models/categoria.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, AfterViewInit {
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild('picker') picker!: MatDatepicker<any>;

  transacoes: Transacao[] = [];
  categorias: Categoria[] = []; 
  transacoesFiltradas: Transacao[] = [];
  isLoading = false;
  error: string | null = null;
  dataSource: MatTableDataSource<Transacao> = new MatTableDataSource<Transacao>();
  displayedColumns: string[] = ['data', 'descricao', 'tipo', 'valor'];

  filterForm: FormGroup;
  barChartData = {};
  lineChartData = {};
  pieChartData = {};
  chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { 
        ticks: {
          autoSkip: true,
          maxTicksLimit: 20
        }
      }
    }
  };

  constructor(private transacaoService: TransacaoService, private fb: FormBuilder) {
    this.filterForm = this.fb.group({
      mesAno: new FormControl(new Date()) 
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  loadData(): void {
    this.isLoading = true;
    this.transacaoService.getTransacoes().subscribe(
      (data) => {
        this.transacoes = data;
        this.transacoesFiltradas = data;
        this.dataSource.data = this.transacoesFiltradas;
        this.error = null;
        this.isLoading = false;
        this.setupCharts();
        this.aplicarFiltro(); 
      },
      (err) => {
        this.error = 'Erro ao carregar as transações.';
        this.isLoading = false;
      }
    );
  }

  setMonthAndYear(normalizedMonthAndYear: Date) {
    const ctrlValue = this.filterForm.get('mesAno')?.value;
    const ctrlDate = new Date(ctrlValue);
    ctrlDate.setMonth(normalizedMonthAndYear.getMonth());
    ctrlDate.setFullYear(normalizedMonthAndYear.getFullYear());
    this.filterForm.get('mesAno')?.setValue(ctrlDate);
    this.picker.close();
    this.aplicarFiltro(); 
  }

  setupCharts(): void {
    const dadosFiltrados = this.transacoesFiltradas;
    
    const entradas = dadosFiltrados
      .filter(t => t.tipo === 'Entrada')
      .reduce((sum, t) => sum + t.valor, 0);
    
    const saidas = dadosFiltrados
      .filter(t => t.tipo === 'Saida')
      .reduce((sum, t) => sum + t.valor, 0);

    this.barChartData = {
      labels: [this.getMesAtual()],
      datasets: [
        {
          label: 'Entradas',
          data: [entradas],
          backgroundColor: 'rgba(0, 255, 0, 0.2)',
        },
        {
          label: 'Saidas',
          data: [saidas],
          backgroundColor: 'rgba(255, 0, 0, 0.2)',
        },
      ],
    };

    const saldoDiario = dadosFiltrados
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

    this.lineChartData = {
      labels: saldoDiario.map(item => item.data),
      datasets: [
        {
          label: 'Evolução do Saldo',
          data: saldoDiario.map(item => item.saldo),
          borderColor: 'rgba(0, 255, 255, 0.8)',
          fill: false,
        },
      ],
    };

    const despesasPorCategoria = dadosFiltrados
      .filter(t => t.tipo === 'Saida')
      .reduce((acc: any, t) => {
        acc[t.categoria.nome] = (acc[t.categoria.nome] || 0) + t.valor;
        return acc;
      }, {});

    this.pieChartData = {
      labels: Object.keys(despesasPorCategoria),
      datasets: [
        {
          data: Object.values(despesasPorCategoria),
          backgroundColor: ['#ff9999', '#66b3ff', '#99ff99', '#ffcc99'],
        },
      ],
    };
  }

  getTotalEntradas(): number {
    return this.transacoesFiltradas
      .filter((t) => t.tipo === 'Entrada')
      .reduce((sum, t) => sum + t.valor, 0);
  }

  getTotalSaidas(): number {
    return this.transacoesFiltradas
      .filter((t) => t.tipo === 'Saida')
      .reduce((sum, t) => sum + t.valor, 0);
  }

  getSaldo(): number {
    return this.getTotalEntradas() - this.getTotalSaidas();
  }

  getMediaMensal(): number {
    const total = this.transacoesFiltradas.reduce((sum, t) => sum + t.valor, 0);
    return total / 12;
  }

  getMesAtual(): string {
    const mesAnoSelecionado = this.filterForm.get('mesAno')?.value || new Date();
    const months = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
      'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    return months[mesAnoSelecionado.getMonth()];
  }

  reloadData(): void {
    this.loadData();
  }

  aplicarFiltro(): void {
    if (this.filterForm.valid) {
      const mesAnoSelecionado = this.filterForm.get('mesAno')?.value;
      
      if (mesAnoSelecionado) {
        const mes = mesAnoSelecionado.getMonth();
        const ano = mesAnoSelecionado.getFullYear();

        this.transacoesFiltradas = this.transacoes.filter(t => {
          const transacaoDate = new Date(t.data);
          return transacaoDate.getMonth() === mes && 
                 transacaoDate.getFullYear() === ano;
        });

        this.dataSource.data = this.transacoesFiltradas;
        this.setupCharts(); 
      }
    }
  }

  aplicarFiltroTabela(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  public pieChartOptions: any = {
    responsive: true,
    maintainAspectRatio: false,
    aspectRatio: 1,
    plugins: {
      legend: {
        position: 'top',
        labels: {
          font: {
            size: 12,
          },
          padding: 10,
        }
      },
      tooltip: {
        callbacks: {
          label: function(tooltipItem: { raw: any; }) {
            return `R$ ${tooltipItem.raw.toFixed(2)}`;
          }
        }
      }
    },
    cutout: '60%',
  };
}