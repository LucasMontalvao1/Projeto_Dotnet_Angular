import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LembreteService } from '../../services/lembrete.service';
import { LembreteWebSocketService } from '../../services/lembrete-websocket.service';
import { User } from '../../models/User';
import { Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { PageEvent } from '@angular/material/paginator';
import { Lembrete } from '../../models/lembrete.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { LembretesComponent } from '../lembretes/lembretes.component';
import { LembretesExcluirComponent } from '../lembretes/lembretes-excluir/lembretes-excluir.component';
import { LembretesDetalhesComponent } from '../lembretes/lembretes-detalhes/lembretes-detalhes.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy {
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  user: User | null = null;
  selectedDate: Date = new Date();
  lembretes: Lembrete[] = [];
  filteredLembretes: Lembrete[] = [];
  displayedLembretes: Lembrete[] = [];
  pageSize: number = 3;
  pageIndex: number = 0;
  private lembreteSubscription: Subscription | undefined;
  private isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private lembreteService: LembreteService,
    private lembreteWebSocketService: LembreteWebSocketService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.checkAuthentication();
    this.subscribeToLembretes();
    this.carregaLembretes();
  }

  ngOnDestroy(): void {
    this.lembreteWebSocketService.disconnect();
    if (this.lembreteSubscription) {
      this.lembreteSubscription.unsubscribe();
    }
  }

  checkAuthentication(): void {
    const decodedToken = this.authService.getDecodedToken();
    const token = this.authService.getToken();

    if (decodedToken && token) {
      this.user = {
        usuarioID: decodedToken.nameid,
        username: decodedToken.given_name,
        name: decodedToken.unique_name,
        email: decodedToken.email,
        foto: decodedToken.Foto,
        token: token
      };
      this.carregaLembretes();
    } else {
      this.router.navigate(['/login']);
    }
  }

  carregaLembretes(): void {
    if (this.isLoading) return;
    
    this.isLoading = true;
    this.openSnackBar('Carregando lembretes...', 'Fechar');
    
    this.lembreteService.getLembretes().subscribe({
      next: (data: Lembrete[]) => {
        this.lembretes = data.map(lembrete => ({
          ...lembrete,
          dataLembrete: new Date(lembrete.dataLembrete)
        }));
        this.filtraLembretes();
        this.openSnackBar('Lembretes carregados com sucesso!', 'Fechar');
      },
      error: (error) => {
        console.error('Erro ao buscar lembretes:', error);
        this.openSnackBar('Erro ao buscar lembretes', 'Fechar');
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  filtraLembretes(): void {
    // Cria uma nova data para não mutar a selectedDate
    const compareDate = new Date(this.selectedDate);
    compareDate.setHours(0, 0, 0, 0);

    this.filteredLembretes = this.lembretes.filter(lembrete => {
      const lembreteDate = new Date(lembrete.dataLembrete);
      lembreteDate.setHours(0, 0, 0, 0);
      return lembreteDate.getTime() === compareDate.getTime();
    });

    if (this.paginator) {
      this.paginator.length = this.filteredLembretes.length;
      this.paginator.pageIndex = 0; // Reset para primeira página ao filtrar
    }
    this.pageIndex = 0;
    this.updateDisplayedLembretes();
  }

  updateDisplayedLembretes(): void {
    const startIndex = this.pageIndex * this.pageSize;
    this.displayedLembretes = this.filteredLembretes.slice(startIndex, startIndex + this.pageSize);
    console.log('Lembretes exibidos:', this.displayedLembretes);
  }

  onDateSelected(date: Date): void {
    this.selectedDate = new Date(date); 
    this.filtraLembretes();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updateDisplayedLembretes();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  openSnackBar(message: string, action: string) {
    this.snackBar.open(message, action, {
      duration: 3000,
    });
  }

  subscribeToLembretes(): void {
    this.lembreteSubscription = this.lembreteWebSocketService.getLembretes().subscribe({
      next: (lembrete: Lembrete) => {
        // Adiciona o novo lembrete e atualiza a visualização
        const novoLembrete = { ...lembrete, dataLembrete: new Date(lembrete.dataLembrete) };
        this.lembretes = [...this.lembretes, novoLembrete];
        this.filtraLembretes();
        this.openSnackBar('Novo lembrete recebido!', 'Fechar');
        this.carregaLembretes();
      },
      error: (error) => {
        console.error('Erro ao receber lembretes via WebSocket:', error);
        this.openSnackBar('Erro ao receber lembretes', 'Fechar');
      }
    });
  }

  createReminder(): void {
    const dialogRef = this.dialog.open(LembretesComponent);
    
    dialogRef.afterClosed().subscribe(result => {
        this.carregaLembretes();
    });
  }

  editReminder(reminder: Lembrete): void {
    const reminderToEdit = {
      ...reminder,
      dataLembrete: new Date(reminder.dataLembrete)
    };

    const dialogRef = this.dialog.open(LembretesComponent, { 
      data: reminderToEdit
    });

    dialogRef.afterClosed().subscribe(result => {
        this.carregaLembretes();
    });
  }


  deleteReminder(reminder: Lembrete): void {
    const dialogRef = this.dialog.open(LembretesExcluirComponent, {
      data: reminder,
      width: 'max-content', 
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregaLembretes(); 
      }
    });
  }

  viewDetails(reminder: Lembrete): void {
    this.dialog.open(LembretesDetalhesComponent, { data: reminder });
  }
}

