import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
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
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
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
  totalItems: number = 0;
  private lembreteSubscription: Subscription | undefined;
  isLoading: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private lembreteService: LembreteService,
    private lembreteWebSocketService: LembreteWebSocketService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private cdr: ChangeDetectorRef
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

  private handleError(error: any): void {
    let errorMessage = 'Ocorreu um erro';
    if (error.error?.message) {
      errorMessage = error.error.message;
    } else if (error.message) {
      errorMessage = error.message;
    }
    this.openSnackBar(errorMessage, 'Fechar');
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
    this.cdr.detectChanges();
    
    this.lembreteService.getLembretesByUser()
      .pipe(debounceTime(300))
      .subscribe({
        next: (data: Lembrete[]) => {
          this.lembretes = data.map(lembrete => ({
            ...lembrete,
            dataLembrete: new Date(lembrete.dataLembrete)
          }));
          this.filtraLembretes();
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.handleError(error);
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        complete: () => {
          this.isLoading = false;
          this.cdr.detectChanges();
        }
    });
  }

  filtraLembretes(): void {
    const compareDate = new Date(this.selectedDate);
    compareDate.setHours(0, 0, 0, 0);

    this.filteredLembretes = this.lembretes.filter(lembrete => {
      const lembreteDate = new Date(lembrete.dataLembrete);
      lembreteDate.setHours(0, 0, 0, 0);
      return lembreteDate.getTime() === compareDate.getTime();
    });

    this.totalItems = this.filteredLembretes.length;
    
    if (this.paginator) {
      this.paginator.length = this.totalItems;
      this.paginator.pageIndex = 0;
    }
    
    this.pageIndex = 0;
    this.updateDisplayedLembretes();
  }

  updateDisplayedLembretes(): void {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.displayedLembretes = this.filteredLembretes.slice(startIndex, endIndex);
    this.cdr.detectChanges();
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
      horizontalPosition: 'end',
      verticalPosition: 'bottom'
    });
  }

  subscribeToLembretes(): void {
    this.lembreteSubscription = this.lembreteWebSocketService.getLembretes()
      .pipe(debounceTime(300))
      .subscribe({
        next: (lembrete: Lembrete) => {
          const novoLembrete = { ...lembrete, dataLembrete: new Date(lembrete.dataLembrete) };
          this.lembretes = [...this.lembretes, novoLembrete];
          this.filtraLembretes();
          this.openSnackBar('Novo lembrete recebido!', 'Fechar');
          this.carregaLembretes();
          this.cdr.detectChanges();
        },
        error: (error) => {
          this.handleError(error);
        }
    });
  }

  createReminder(): void {
    const dialogRef = this.dialog.open(LembretesComponent, {
      width: '650px',
      disableClose: true
    });
    
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregaLembretes();
      }
    });
  }

  editReminder(reminder: Lembrete): void {
    const reminderToEdit = {
      ...reminder,
      dataLembrete: new Date(reminder.dataLembrete)
    };

    const dialogRef = this.dialog.open(LembretesComponent, {
      width: '650px',
      data: reminderToEdit,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregaLembretes();
      }
    });
  }

  deleteReminder(reminder: Lembrete): void {
    const dialogRef = this.dialog.open(LembretesExcluirComponent, {
      data: reminder,
      width: '650px',
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.carregaLembretes();
      }
    });
  }

  viewDetails(reminder: Lembrete): void {
    this.dialog.open(LembretesDetalhesComponent, {
      data: reminder,
      width: '600px'
    });
  }
}