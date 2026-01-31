import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TransacaoService } from '../../../../services/transacao.service';
import { CategoriaService } from '../../../../services/categoria.service';
import { Transacao } from '../../../../models/transacao.model';
import { Categoria } from '@/app/models/categoria.model';

@Component({
  selector: 'app-transacao-edit',
  templateUrl: './transacao-edit.component.html',
  styleUrls: ['./transacao-edit.component.css']
})
export class TransacaoEditComponent implements OnInit {
  form: FormGroup;
  isLoading = false;
  error: string | null = null;
  categorias: Categoria[] = [];

  constructor(
    private fb: FormBuilder,
    private transacaoService: TransacaoService,
    private categoriaService: CategoriaService,
    private dialogRef: MatDialogRef<TransacaoEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Transacao
  ) {
    console.log('Dados recebidos para edição:', data); // Debug

    this.form = this.fb.group({
      transacaoID: [data.transacaoID],
      usuarioID: [data.usuarioID],
      categoriaID: [data.categoriaID, Validators.required],
      data: [new Date(data.data), Validators.required],
      valor: [data.valor, [Validators.required, Validators.min(0.01)]],
      tipo: [data.tipo, Validators.required],
      descricao: [data.descricao, [Validators.required, Validators.minLength(3)]]
    });
  }

  ngOnInit(): void {
    this.loadCategorias();
  }

  loadCategorias(): void {
    this.categoriaService.getCategorias().subscribe({
      next: (categorias) => {
        this.categorias = categorias;
      },
      error: (error) => {
        console.error('Erro ao carregar categorias:', error);
        this.error = 'Erro ao carregar categorias';
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid && !this.isLoading) {
      this.isLoading = true;

      // Criar objeto transação mantendo dados importantes
      const transacao: Transacao = {
        ...this.form.value,
        criadoEm: this.data.criadoEm, // Mantém a data de criação original
        categoriaID: parseInt(this.form.value.categoriaID),
        usuarioID: parseInt(this.form.value.usuarioID)
      };
      
      console.log('Transação a ser atualizada:', transacao); // Debug

      this.transacaoService.updateTransacao(transacao).subscribe({
        next: (response) => {
          console.log('Transação atualizada com sucesso:', response);
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao atualizar transação:', error);
          this.error = error.message;
          this.isLoading = false;
        }
      });
    }
  }
}