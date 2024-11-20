import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoriaService } from '../../../services/categoria.service';
import { Categoria } from '../../../models/categoria.model';
import { CategoriaCreateComponent } from '../categoria-create/categoria-create.component';
import { CategoriaEditComponent } from '../categoria-edit/categoria-edit.component';
import { CategoriaDetailComponent } from '../categoria-detail/categoria-detail.component';
import { CategoriaDeleteComponent } from '../categoria-delete/categoria-delete.component';

@Component({
  selector: 'app-categoria-list',
  templateUrl: './categoria-list.component.html',
  styleUrls: ['./categoria-list.component.css']
})
export class CategoriaListComponent implements OnInit {
  displayedColumns: string[] = ['categoriaID', 'nome', 'descricao', 'acoes'];
  dataSource: MatTableDataSource<Categoria>;
  isLoading = true;
  error: string | null = null;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private categoriaService: CategoriaService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Categoria>();
  }

  ngOnInit(): void {
    this.loadCategorias();
  }

  loadCategorias(): void {
    this.isLoading = true;
    this.categoriaService.getCategorias().subscribe({
      next: (categorias) => {
        this.dataSource.data = categorias;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (error) => {
        this.error = error.message;
        this.isLoading = false;
        this.showMessage('Erro ao carregar categorias', true);
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CategoriaCreateComponent, {
      width: '600px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategorias();
        this.showMessage('Categoria criada com sucesso');
      }
    });
  }

  openEditDialog(categoria: Categoria): void {
    const dialogRef = this.dialog.open(CategoriaEditComponent, {
      width: '600px',
      data: categoria
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategorias();
        this.showMessage('Categoria atualizada com sucesso');
      }
    });
  }

  openDetailsDialog(categoria: Categoria): void {
    this.dialog.open(CategoriaDetailComponent, {
      width: '600px',
      data: categoria
    });
  }

  openDeleteDialog(categoria: Categoria): void {
    const dialogRef = this.dialog.open(CategoriaDeleteComponent, {
      width: '400px',
      data: categoria
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadCategorias();
        this.showMessage('Categoria excluída com sucesso');
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
