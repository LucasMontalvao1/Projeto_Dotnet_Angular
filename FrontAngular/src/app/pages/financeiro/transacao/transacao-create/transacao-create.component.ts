import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { TransacaoService } from '../../../../services/transacao.service';
import { CategoriaService } from '../../../../services/categoria.service';
import { TransacaoDto } from '../../../../models/transacao-dto.model';
import { AuthService } from '../../../../services/auth.service'; 
import { Categoria } from '@/app/models/categoria.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-transacao-create',
  templateUrl: './transacao-create.component.html',
  styleUrls: ['./transacao-create.component.css']
})
export class TransacaoCreateComponent implements OnInit {
  form: FormGroup;
  isLoading = false;
  error: string | null = null;
  categorias: Categoria[] = []; 
  transacao: TransacaoDto;

  constructor(
    private fb: FormBuilder,
    private transacaoService: TransacaoService,
    private categoriaService: CategoriaService,
    private authService: AuthService,
    private dialogRef: MatDialogRef<TransacaoCreateComponent>
  ) {
    const decodedToken = this.authService.getDecodedToken();

    // Inicializar a transação com valores padrão
    this.transacao = {
      usuarioID: parseInt(decodedToken.nameid), // Converter para número
      categoriaID: 0,
      tipo: '',
      valor: 0,
      descricao: '',
      data: new Date(),
      criadoEm: new Date()
    };

    this.form = this.fb.group({
      categoriaID: ['', Validators.required],
      data: ['', Validators.required],
      valor: ['', [Validators.required, Validators.min(0.01)]],
      tipo: ['', Validators.required],
      descricao: ['', [Validators.required, Validators.minLength(3)]]
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
        this.error = 'Erro ao carregar categorias';
        console.error(error);
      }
    });
  }

  onSubmit(): void {
    if (this.form.valid && !this.isLoading) {
      this.isLoading = true;
      
      // Atualizar o objeto transacao com os valores do formulário
      this.transacao = {
        ...this.transacao,
        ...this.form.value,
        usuarioID: parseInt(this.authService.getDecodedToken().nameid),
        categoriaID: parseInt(this.form.value.categoriaID),
        valor: parseFloat(this.form.value.valor)
      };
      
      console.log('Transação a ser enviada:', this.transacao); // Para debug

      this.transacaoService.createTransacao(this.transacao).subscribe({
        next: (response) => {
          console.log('Transação criada com sucesso:', response);
          this.dialogRef.close(true);
        },
        error: (error) => {
          console.error('Erro ao criar transação:', error);
          this.error = error.message;
          this.isLoading = false;
        }
      });
    }
  }
}