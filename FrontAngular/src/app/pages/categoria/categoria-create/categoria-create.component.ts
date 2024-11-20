import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { CategoriaService } from '../../../services/categoria.service';
import { Categoria } from '@/app/models/categoria.model';

@Component({
  selector: 'app-categoria-create',
  templateUrl: './categoria-create.component.html',
  styleUrls: ['./categoria-create.component.css']
})
export class CategoriaCreateComponent implements OnInit {
  form: FormGroup;
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private categoriaService: CategoriaService,
    private dialogRef: MatDialogRef<CategoriaCreateComponent>
  ) {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
      descricao: ['', [Validators.maxLength(255)]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.form.valid && !this.isLoading) {
      this.isLoading = true;
      
      const categoria: Categoria = {
        nome: this.form.value.nome,
        descricao: this.form.value.descricao,
        categoriaID: 0,
        ativo: false,
        usuarioID: 0
      };
      
      this.categoriaService.createCategoria(categoria).subscribe({
        next: (response) => {
          console.log('Categoria criada com sucesso:', response);
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao criar categoria:', error);
          this.error = error.message;
          this.isLoading = false;
        }
      });
    }
  }
}
