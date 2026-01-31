import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LembreteService } from '@/app/services/lembrete.service'; 
import { Lembrete } from '@/app/models/lembrete.model'; 

@Component({
  selector: 'app-lembretes-excluir',
  templateUrl: './lembretes-excluir.component.html',
  styleUrls: ['./lembretes-excluir.component.css']
})
export class LembretesExcluirComponent {
  constructor(
    public dialogRef: MatDialogRef<LembretesExcluirComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Lembrete,
    private lembreteService: LembreteService,
    private snackBar: MatSnackBar
  ) {}

  onConfirm(): void {
    const lembreteId = this.data.lembreteID;
  
    if (lembreteId !== undefined) {
      this.lembreteService.deleteLembrete(lembreteId).subscribe({
        next: () => {
          this.dialogRef.close(true);
          this.snackBar.open('Lembrete excluído com sucesso!', 'Fechar', {
            duration: 3000,
            verticalPosition: 'top',
            horizontalPosition: 'right',
          });
        },
        error: (error) => {
          console.error('Erro ao excluir lembrete:', error);
          this.snackBar.open('Erro ao excluir lembrete. Tente novamente.', 'Fechar', {
            duration: 3000,
            verticalPosition: 'top',
            horizontalPosition: 'right',
          });
        }
      });
    } else {
      console.error('Erro: ID do lembrete não está definido.');
      this.snackBar.open('Erro: ID do lembrete não está disponível.', 'Fechar', {
        duration: 3000,
        verticalPosition: 'top',
        horizontalPosition: 'right',
      });
    }
  }
  

  onCancel(): void {
    this.dialogRef.close();
  }
}
