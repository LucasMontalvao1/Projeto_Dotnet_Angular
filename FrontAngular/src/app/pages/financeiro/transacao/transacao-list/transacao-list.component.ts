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

// Components
import { TransacaoCreateComponent } from '../transacao-create/transacao-create.component';
import { TransacaoEditComponent } from '../transacao-edit/transacao-edit.component';
import { TransacaoDetailComponent } from '../transacao-detail/transacao-detail.component';
import { TransacaoDeleteComponent } from '../transacao-delete/transacao-delete.component';

interface ResumoFinanceiro {
  totalReceitas: number;
  totalDespesas: number;
  saldoTotal: number;
}

@Component({
  selector: 'app-transacao-list',
  templateUrl: './transacao-list.component.html',
  styleUrls: ['./transacao-list.component.css']
})
export class TransacaoListComponent implements OnInit, OnDestroy {
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

  // Resumo Financeiro
  resumo: ResumoFinanceiro = {
    totalReceitas: 0,
    totalDespesas: 0,
    saldoTotal: 0
  };

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

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

  loadTransacoes(): void {
    this.isLoading = true;
    this.transacaoService.getTransacoes()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (transacoes) => {
          console.log('Transações recebidas:', transacoes);
          this.dataSource.data = transacoes;
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.calcularResumoFinanceiro();
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

  private calcularResumoFinanceiro(): void {
    if (!this.dataSource.data) return;

    this.resumo.totalReceitas = this.dataSource.data
      .filter(t => t.tipo === 'Entrada')
      .reduce((acc, curr) => acc + curr.valor, 0);

    this.resumo.totalDespesas = this.dataSource.data
      .filter(t => t.tipo === 'Saida')
      .reduce((acc, curr) => acc + curr.valor, 0);

    this.resumo.saldoTotal = this.resumo.totalReceitas - this.resumo.totalDespesas;
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

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

  private prepararDadosExportacao() {
    return this.dataSource.filteredData.map(linha => ({
      'ID': linha.transacaoID,
      'Data': new Date(linha.data).toLocaleDateString('pt-BR'),
      'Valor': new Intl.NumberFormat('pt-BR', { 
        style: 'currency', 
        currency: 'BRL' 
      }).format(linha.valor),
      'Tipo': linha.tipo,
      'Categoria': linha.categoria.nome,
      'Descrição': linha.descricao,
      'Criado em': new Date(linha.criadoEm).toLocaleDateString('pt-BR')
    }));
  }

  private configurarCabecalhoPDF(doc: jsPDF): void {
    doc.setFontSize(16);
    doc.text('Relatório de Transações', 14, 15);
    doc.setFontSize(10);
    doc.text(`Período: ${this.getDataFormatada()}`, 14, 22);
    
    // Adiciona informações do resumo financeiro
    doc.text(`Total Receitas: ${new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(this.resumo.totalReceitas)}`, 14, 29);
    
    doc.text(`Total Despesas: ${new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(this.resumo.totalDespesas)}`, 14, 36);
    
    doc.text(`Saldo: ${new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(this.resumo.saldoTotal)}`, 14, 43);
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
        const rodape = `Gerado em: ${new Date().toLocaleDateString('pt-BR')} ${new Date().toLocaleTimeString('pt-BR')}`;
        doc.setFontSize(8);
        doc.text(rodape, data.settings.margin.left, doc.internal.pageSize.height - 10);
      }
    });
  }

  private getDataFormatada(): string {
    return new Date().toISOString().split('T')[0];
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