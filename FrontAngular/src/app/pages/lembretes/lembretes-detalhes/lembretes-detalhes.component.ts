import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Lembrete } from '@/app/models/lembrete.model';

@Component({
  selector: 'app-lembretes-detalhes',
  templateUrl: './lembretes-detalhes.component.html',
  styleUrls: ['./lembretes-detalhes.component.css']
})
export class LembretesDetalhesComponent {
  constructor(
    public dialogRef: MatDialogRef<LembretesDetalhesComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Lembrete
  ) {}

  onClose(): void {
    this.dialogRef.close(); 
  }
}
