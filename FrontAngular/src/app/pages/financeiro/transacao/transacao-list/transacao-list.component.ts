import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TransacaoService } from '../../../../services/transacao.service';
import { Transacao } from '../../../../models/transacao.model';
import { TransacaoCreateComponent } from '../transacao-create/transacao-create.component';
import { TransacaoEditComponent } from '../transacao-edit/transacao-edit.component';
import { TransacaoDetailComponent } from '../transacao-detail/transacao-detail.component';
import { TransacaoDeleteComponent } from '../transacao-delete/transacao-delete.component';

@Component({
  selector: 'app-transacao-list',
  templateUrl: './transacao-list.component.html',
  styleUrls: ['./transacao-list.component.css']
})
export class TransacaoListComponent implements OnInit {
  displayedColumns: string[] = ['transacaoID', 'data', 'valor', 'tipo', 'descricao', 'categoria', 'acoes'];
  dataSource: MatTableDataSource<Transacao>;
  isLoading = true;
  error: string | null = null;

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
  }

  loadTransacoes(): void {
    this.isLoading = true;
    this.transacaoService.getTransacoes().subscribe({
      next: (transacoes) => {
        this.dataSource.data = transacoes;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (error) => {
        this.error = error.message;
        this.isLoading = false;
        this.showMessage('Erro ao carregar transações', true);
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(TransacaoCreateComponent, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTransacoes();
        this.showMessage('Transação criada com sucesso');
      }
    });
  }

  openEditDialog(transacao: Transacao): void {
    const dialogRef = this.dialog.open(TransacaoEditComponent, {
      width: '600px',
      data: transacao
    });

    dialogRef.afterClosed().subscribe(result => {
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
      data: transacao
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadTransacoes();
        this.showMessage('Transação excluída com sucesso');
      }
    });
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