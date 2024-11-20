import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CategoriaService } from '../../../services/categoria.service';
import { Categoria } from '@/app/models/categoria.model';

@Component({
  selector: 'app-categoria-edit',
  templateUrl: './categoria-edit.component.html',
  styleUrls: ['./categoria-edit.component.css']
})
export class CategoriaEditComponent implements OnInit {
  form: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private categoriaService: CategoriaService,
    private dialogRef: MatDialogRef<CategoriaEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { categoriaID: number } // Recebe o ID da categoria
  ) {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      descricao: ['', [Validators.maxLength(255)]]
    });
  }

  ngOnInit(): void {
    this.loadCategoria();
  }

  loadCategoria(): void {
    this.isLoading = true;
    this.categoriaService.getCategoriaById(this.data.categoriaID).subscribe({
      next: (categoria) => {
        this.form.patchValue(categoria); // Preenche os campos do formulário com os dados recebidos
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar categoria:', error);
        this.error = 'Erro ao carregar dados da categoria';
        this.isLoading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid && !this.isLoading) {
      this.isLoading = true;

      const categoriaAtualizada: Categoria = {
        ...this.form.value,
        categoriaID: this.data.categoriaID // Mantém o ID da categoria
      };

      this.categoriaService.updateCategoria(categoriaAtualizada).subscribe({
        next: (response) => {
          console.log('Categoria editada com sucesso:', response);
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao editar categoria:', error);
          this.error = error.message;
          this.isLoading = false;
        }
      });
    }
  }
}
