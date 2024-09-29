import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { LembreteService } from '../../services/lembrete.service';
import { User } from '../../models/User';
import { Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { PageEvent } from '@angular/material/paginator';
import { Lembrete } from '@/app/models/lembrete.model';
import { MatSnackBar } from '@angular/material/snack-bar'; // Importando MatSnackBar

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  user: User | null = null;
  selectedDate: Date = new Date();
  lembretes: Lembrete[] = [];
  filteredLembretes: Lembrete[] = [];
  displayedLembretes: Lembrete[] = [];
  pageSize: number = 5;
  pageIndex: number = 0;

  constructor(
    private authService: AuthService,
    private router: Router,
    private lembreteService: LembreteService,
    private snackBar: MatSnackBar // Adicionando MatSnackBar ao construtor
  ) { }

  ngOnInit(): void {
    this.checkAuthentication();
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
    this.lembreteService.getLembretes().subscribe(
      (data: Lembrete[]) => {
        this.lembretes = data.map(lembrete => ({
          ...lembrete,
          dataLembrete: new Date(lembrete.dataLembrete)
        }));
        this.filtraLembretes();
      },
      (error) => {
        this.openSnackBar('Erro ao buscar lembretes', 'Fechar'); // Exibindo erro ao usuário
      }
    );
  }

  filtraLembretes(): void {
    const selectedTime = this.selectedDate.setHours(0, 0, 0, 0);

    this.filteredLembretes = this.lembretes.filter(lembrete => {
      const lembreteTime = new Date(lembrete.dataLembrete).setHours(0, 0, 0, 0);
      return lembreteTime === selectedTime;
    });

    if (this.paginator) {
      this.paginator.length = this.filteredLembretes.length;
    }
    this.pageIndex = 0;
    this.updateDisplayedLembretes();
  }

  updateDisplayedLembretes(): void {
    const startIndex = this.pageIndex * this.pageSize;
    this.displayedLembretes = this.filteredLembretes.slice(startIndex, startIndex + this.pageSize);
  }

  onDateSelected(date: Date): void {
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

  // Funções de lembretes (a serem implementadas conforme necessidade)
  createReminder(): void {
    // Abrir um diálogo para criar um novo lembrete
  }

  editReminder(reminder: Lembrete): void {
    // Abrir um diálogo para editar o lembrete existente
  }

  viewDetails(reminder: Lembrete): void {
    // Abrir um diálogo para visualizar os detalhes do lembrete
  }

  deleteReminder(reminder: Lembrete): void {
    // Excluir o lembrete selecionado
  }
}
