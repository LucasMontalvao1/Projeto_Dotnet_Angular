import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TransacaoService } from '../../../../services/transacao.service';
import { Transacao } from '../../../../models/transacao.model';

@Component({
  selector: 'app-transacao-delete',
  templateUrl: './transacao-delete.component.html',
  styleUrls: ['./transacao-delete.component.css']
})
export class TransacaoDeleteComponent {
  isLoading = false;
  error: string | null = null;

  constructor(
    private transacaoService: TransacaoService,
    private dialogRef: MatDialogRef<TransacaoDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Transacao
  ) {}

  confirm(): void {
    if (!this.isLoading) {
      this.isLoading = true;
      
      this.transacaoService.deleteTransacao(this.data.transacaoID).subscribe({
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