import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CategoriaService } from '../../../services/categoria.service';
import { Categoria } from '../../../models/categoria.model';

@Component({
  selector: 'app-categoria-delete',
  templateUrl: './categoria-delete.component.html',
  styleUrls: ['./categoria-delete.component.css']
})
export class CategoriaDeleteComponent {
  isLoading = false;
  error: string | null = null;

  constructor(
    private categoriaService: CategoriaService,
    private dialogRef: MatDialogRef<CategoriaDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Categoria
  ) {}

  confirm(): void {
    if (!this.isLoading) {
      this.isLoading = true;
      
      this.categoriaService.deleteCategoria(this.data.categoriaID).subscribe({
        next: () => {
          this.dialogRef.close(true);
        },
        error: (error) => {
          this.error = error.message;
          this.isLoading = false;
        }
      });
    }
  }
}
