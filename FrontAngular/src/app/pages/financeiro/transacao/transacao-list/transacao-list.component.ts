import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import * as XLSX from 'xlsx';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

// Services
import { TransacaoService } from '../../../../services/transacao.service';

// Models
import { Transacao } from '../../../../models/transacao.model';
import { TransacaoDto } from '../../../../models/transacao-dto.model';
import { Categoria } from '../../../../models/categoria.model';
import { ResumoFinanceiro } from '@/app/models/resumo-financeiro.model';

// Components
import { TransacaoCreateComponent } from '../transacao-create/transacao-create.component';
import { TransacaoEditComponent } from '../transacao-edit/transacao-edit.component';
import { TransacaoDetailComponent } from '../transacao-detail/transacao-detail.component';
import { TransacaoDeleteComponent } from '../transacao-delete/transacao-delete.component';
import { FiltroData } from '@/app/components/transacao-filtro/transacao-filtro.component';

@Component({
  selector: 'app-transacao-list',
  templateUrl: './transacao-list.component.html',
  styleUrls: ['./transacao-list.component.scss']
})
export class TransacaoListComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  // Propriedades da Tabela
  displayedColumns: string[] = [
    'transacaoID',
    'data',
    'valor',
    'tipo',
    'descricao',
    'categoria',
    'acoes'
  ];
  dataSource: MatTableDataSource<Transacao>;

  // Estados
  isLoading = true;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  // Dados
  transacoesOriginais: Transacao[] = [];
  resumoFinanceiro: ResumoFinanceiro = {
    totalEntradas: 0,
    totalSaidas: 0,
    saldo: 0,
    mediaMensal: 0
  };

  constructor(
    private transacaoService: TransacaoService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Transacao>();
  }

  ngOnInit(): void {
    this.loadTransacoes();
    this.configurarDataSource();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Configuração do DataSource com filtros personalizados
  private configurarDataSource(): void {
    this.dataSource.filterPredicate = (data: Transacao, filter: string) => {
      const searchStr = filter.toLowerCase();
      return data.descricao.toLowerCase().includes(searchStr) ||
             data.categoria.nome.toLowerCase().includes(searchStr) ||
             data.tipo.toLowerCase().includes(searchStr);
    };

    this.dataSource.sortingDataAccessor = (data: Transacao, sortHeaderId: string): string | number => {
      switch (sortHeaderId) {
        case 'categoria': return data.categoria.nome;
        case 'data': return new Date(data.data).getTime();
        case 'valor': return data.valor;
        case 'criadoEm': return new Date(data.criadoEm).getTime();
        default: return data[sortHeaderId as keyof Transacao]?.toString() || '';
      }
    };
  }

  // Carregamento inicial das transações
  loadTransacoes(): void {
    this.isLoading = true;
    this.transacaoService.getTransacoes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (transacoes) => {
          this.transacoesOriginais = transacoes;
          this.dataSource.data = transacoes;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.calcularResumoFinanceiro(transacoes);
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao carregar transações:', error);
          this.error = error.message;
          this.isLoading = false;
          this.showMessage('Erro ao carregar transações', true);
        }
      });
  }

  // Handler do filtro de data
  onFiltroAplicado(filtro: FiltroData): void {
    let transacoesFiltradas = [...this.transacoesOriginais];

    if (filtro.dataInicio && filtro.dataFim) {
      const inicio = new Date(filtro.dataInicio);
      const fim = new Date(filtro.dataFim);
      fim.setHours(23, 59, 59);

      transacoesFiltradas = this.transacoesOriginais.filter(transacao => {
        const dataTransacao = new Date(transacao.data);
        return dataTransacao >= inicio && dataTransacao <= fim;
      });
    }

    this.dataSource.data = transacoesFiltradas;
    this.calcularResumoFinanceiro(transacoesFiltradas);

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  // Cálculo do resumo financeiro
  private calcularResumoFinanceiro(transacoes: Transacao[]): void {
    const totalEntradas = transacoes
      .filter(t => t.tipo === 'Entrada')
      .reduce((sum, t) => sum + t.valor, 0);

    const totalSaidas = transacoes
      .filter(t => t.tipo === 'Saida')
      .reduce((sum, t) => sum + t.valor, 0);

    this.resumoFinanceiro = {
      totalEntradas,
      totalSaidas,
      saldo: totalEntradas - totalSaidas,
      mediaMensal: transacoes.length > 0 ? 
        (totalEntradas - totalSaidas) / 12 : 0
    };
  }

  // Filtro da tabela
  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  // CRUD Operations
  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TransacaoCreateComponent, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        if (result) {
          this.loadTransacoes();
          this.showMessage('Transação criada com sucesso');
        }
      });
  }

  openEditDialog(transacao: Transacao): void {
    const dialogRef = this.dialog.open(TransacaoEditComponent, {
      width: '600px',
      data: transacao,
      disableClose: true
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        if (result) {
          this.loadTransacoes();
          this.showMessage('Transação atualizada com sucesso');
        }
      });
  }

  openDetailsDialog(transacao: Transacao): void {
    this.dialog.open(TransacaoDetailComponent, {
      width: '600px',
      data: transacao
    });
  }

  openDeleteDialog(transacao: Transacao): void {
    const dialogRef = this.dialog.open(TransacaoDeleteComponent, {
      width: '400px',
      data: transacao,
      disableClose: true
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        if (result) {
          this.loadTransacoes();
          this.showMessage('Transação excluída com sucesso');
        }
      });
  }

  // Export Methods
  exportarParaExcel(): void {
    const dadosExportacao = this.prepararDadosExportacao();
    const planilha = XLSX.utils.json_to_sheet(dadosExportacao);
    const pastaTrabalho = XLSX.utils.book_new();
    
    XLSX.utils.book_append_sheet(pastaTrabalho, planilha, 'Transações');
    XLSX.writeFile(pastaTrabalho, `transacoes_${this.getDataFormatada()}.xlsx`);
    
    this.showMessage('Arquivo Excel gerado com sucesso');
  }

  exportarParaPDF(): void {
    const doc = new jsPDF();
    const dadosExportacao = this.prepararDadosExportacao();

    this.configurarCabecalhoPDF(doc);
    this.adicionarTabelaPDF(doc, dadosExportacao);
    
    doc.save(`transacoes_${this.getDataFormatada()}.pdf`);
    this.showMessage('Arquivo PDF gerado com sucesso');
  }

  // Export Helper Methods
  private prepararDadosExportacao() {
    return this.dataSource.filteredData.map(linha => ({
      'ID': linha.transacaoID,
      'Data': this.formatarData(linha.data),
      'Valor': this.formatarMoeda(linha.valor),
      'Tipo': linha.tipo,
      'Categoria': linha.categoria.nome,
      'Descrição': linha.descricao,
      'Criado em': this.formatarData(linha.criadoEm)
    }));
  }

  private configurarCabecalhoPDF(doc: jsPDF): void {
    doc.setFontSize(16);
    doc.text('Relatório de Transações', 14, 15);
    doc.setFontSize(10);
    doc.text(`Data: ${this.formatarData(new Date())}`, 14, 22);
    
    doc.text(`Total Entradas: ${this.formatarMoeda(this.resumoFinanceiro.totalEntradas)}`, 14, 29);
    doc.text(`Total Saídas: ${this.formatarMoeda(this.resumoFinanceiro.totalSaidas)}`, 14, 36);
    doc.text(`Saldo: ${this.formatarMoeda(this.resumoFinanceiro.saldo)}`, 14, 43);
  }

  private adicionarTabelaPDF(doc: jsPDF, dados: any[]): void {
    autoTable(doc, {
      head: [['ID', 'Data', 'Valor', 'Tipo', 'Categoria', 'Descrição', 'Criado em']],
      body: dados.map(Object.values),
      startY: 50,
      styles: {
        fontSize: 8,
        cellPadding: 2,
        font: 'helvetica'
      },
      headStyles: {
        fillColor: [66, 66, 66]
      },
      didDrawPage: (data) => {
        const rodape = `Gerado em: ${this.formatarData(new Date())} ${new Date().toLocaleTimeString('pt-BR')}`;
        doc.setFontSize(8);
        doc.text(rodape, data.settings.margin.left, doc.internal.pageSize.height - 10);
      }
    });
  }

  // Utility Methods
  private getDataFormatada(): string {
    return new Date().toISOString().split('T')[0];
  }

  private formatarData(data: Date | string): string {
    return new Date(data).toLocaleDateString('pt-BR');
  }

  private formatarMoeda(valor: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(valor);
  }

  private showMessage(message: string, isError = false): void {
    this.snackBar.open(message, 'Fechar', {
      duration: 5000,
      panelClass: isError ? ['error-snackbar'] : ['success-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }
}