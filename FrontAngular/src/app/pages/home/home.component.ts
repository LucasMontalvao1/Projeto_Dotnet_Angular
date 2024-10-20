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
  pageSize: number = 5;
  pageIndex: number = 0;
  private lembreteSubscription: Subscription | undefined;

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

  carregaDeleteLembretes(): void {
    this.lembreteService.getLembretes().subscribe(
      (data: Lembrete[]) => {
        this.lembretes = data.map(lembrete => ({
          ...lembrete,
          dataLembrete: new Date(lembrete.dataLembrete)
        }));
        this.filtraLembretes();
      },
      (error) => {
        console.error('Erro ao buscar lembretes:', error);
      }
    );
  }

  carregaLembretes(): void {
    this.openSnackBar('Carregando lembretes...', 'Fechar');
    this.lembreteService.getLembretes().subscribe(
      (data: Lembrete[]) => {
        this.lembretes = data.map(lembrete => ({
          ...lembrete,
          dataLembrete: new Date(lembrete.dataLembrete)
        }));
        this.filtraLembretes();
        this.openSnackBar('Lembretes carregados com sucesso!', 'Fechar');
      },
      (error) => {
        console.error('Erro ao buscar lembretes:', error);
        this.openSnackBar('Erro ao buscar lembretes', 'Fechar');
      }
    );
  }

  filtraLembretes(): void {
    console.log('Filtrando lembretes para a data:', this.selectedDate);
    const selectedTime = this.selectedDate.setHours(0, 0, 0, 0);
    this.filteredLembretes = this.lembretes.filter(lembrete => {
      const lembreteTime = new Date(lembrete.dataLembrete).setHours(0, 0, 0, 0);
      return lembreteTime === selectedTime;
    });

    console.log('Lembretes filtrados:', this.filteredLembretes);
    if (this.paginator) {
      this.paginator.length = this.filteredLembretes.length;
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
    console.log('Data selecionada:', date);
    this.selectedDate = date;
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
    this.lembreteSubscription = this.lembreteWebSocketService.getLembretes().subscribe(
      (lembrete: Lembrete) => {
        console.log('Novo lembrete recebido via WebSocket:', lembrete);
        this.lembretes.push(lembrete);
        this.filtraLembretes();
        this.carregaLembretes();
        this.openSnackBar('Novo lembrete recebido!', 'Fechar');
      },
      (error: any) => {
        console.error('Erro ao receber lembretes via WebSocket:', error);
        this.openSnackBar('Erro ao receber lembretes', 'Fechar');
      }
    );
  }

  createReminder(): void {
    const dialogRef = this.dialog.open(LembretesComponent);
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Dados para criar lembrete:', result);
        this.carregaLembretes(); 
      }
    });
  }

  editReminder(reminder: Lembrete): void {
    console.log('Lembrete a ser editado:', reminder); 

    const dialogRef = this.dialog.open(LembretesComponent, { data: reminder });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Dados para editar lembrete:', result);
        this.carregaLembretes(); 
      }
    });
  }


  deleteReminder(reminder: Lembrete): void {
    const dialogRef = this.dialog.open(LembretesExcluirComponent, {
      data: reminder,
      width: 'max-content', // Define a largura máxima do diálogo
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregaDeleteLembretes(); 
      }
    });
  }

  viewDetails(reminder: Lembrete): void {
    this.dialog.open(LembretesDetalhesComponent, { data: reminder });
  }
}

