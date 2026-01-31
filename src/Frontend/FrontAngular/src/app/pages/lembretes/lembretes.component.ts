import { Lembrete } from '@/app/models/lembrete.model';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '@/app/services/auth.service'; 
import { LembreteService } from '@/app/services/lembrete.service'; 

@Component({
  selector: 'app-lembretes',
  templateUrl: './lembretes.component.html',
  styleUrls: ['./lembretes.component.css']
})
export class LembretesComponent {
  lembrete: Lembrete;
  isSaving: boolean = false; 

  constructor(
    public dialogRef: MatDialogRef<LembretesComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Lembrete,
    private authService: AuthService,
    private lembreteService: LembreteService
  ) {
    const decodedToken = this.authService.getDecodedToken();

    this.lembrete = data 
      ? { ...data } 
      : { 
          lembreteID: undefined, 
          titulo: '', 
          descricao: '', 
          dataLembrete: new Date(), 
          usuarioID: decodedToken.nameid, 
          intervaloEmDias: 0, 
          criadoEm: new Date() 
        };
  }

  onSave(): void {
    if (this.isSaving) return; 
    this.isSaving = true; 

    console.log('Lembrete que será salvo:', this.lembrete);
    
    if (!this.lembrete.lembreteID || this.lembrete.lembreteID === 0) {
        this.lembreteService.createLembrete(this.lembrete).subscribe({
            next: (res) => {
                console.log('Lembrete criado com sucesso:', res);
                this.dialogRef.close(res);
            },
            error: (err) => {
                console.error('Erro ao criar lembrete:', err);
                this.isSaving = false; 
            }
        });
    } else {
        this.lembreteService.editLembrete(this.lembrete).subscribe({
            next: (res) => {
                console.log('Lembrete atualizado com sucesso:', res);
                this.dialogRef.close(res);
            },
            error: (err) => {
                console.error('Erro ao atualizar lembrete:', err);
                this.isSaving = false; 
            }
        });
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}
