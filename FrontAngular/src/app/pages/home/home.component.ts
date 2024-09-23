import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/User';
import { Router } from '@angular/router';
import { MatPaginator } from '@angular/material/paginator';
import { PageEvent } from '@angular/material/paginator';

interface Reminder {
  date: Date; // A data do lembrete
  text: string; // O texto do lembrete
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  user: User | null = null;
  selectedDate: Date = new Date();
  reminders: Reminder[] = [];
  filteredReminders: Reminder[] = [];
  displayedReminders: Reminder[] = [];
  pageSize: number = 5;
  pageIndex: number = 0;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
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
      this.loadReminders();
      this.filterReminders(); // Filtra os lembretes para mostrar apenas os de hoje
    } else {
      this.router.navigate(['/login']);
    }
  }

  loadReminders(): void {
    this.reminders = [
      { date: new Date('2024-09-21'), text: 'Lembrete 1: Reunião às 10h' },
      { date: new Date('2024-09-21'), text: 'Lembrete 2: Entrega de projeto' },
      { date: new Date('2024-09-21'), text: 'Lembrete 3: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 4: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 5: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 6: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 7: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 8: Entrega de projeto' },
      { date: new Date('2024-09-22'), text: 'Lembrete 9: Almoço com o time' },
      { date: new Date('2024-09-22'), text: 'Lembrete 10: Revisão de código' },
    ];
    this.filterReminders(); // Atualiza a lista filtrada inicial
  }

  onDateSelected(date: Date): void {
    console.log('Data selecionada:', date);
    this.selectedDate = date;
    this.filterReminders(); // Filtra lembretes com base na data selecionada
  }

  filterReminders(): void {
    this.filteredReminders = this.reminders.filter(reminder => {
      const reminderDateNormalized = new Date(reminder.date.getFullYear(), reminder.date.getMonth(), reminder.date.getDate());
      const selectedDateNormalized = new Date(this.selectedDate.getFullYear(), this.selectedDate.getMonth(), this.selectedDate.getDate());

      return reminderDateNormalized.getTime() === selectedDateNormalized.getTime();
    });

    this.paginator!.length = this.filteredReminders.length; // Atualiza o comprimento do paginator
    this.pageIndex = 0; // Reseta o índice da página ao filtrar
    this.updateFilteredReminders(); // Atualiza a lista filtrada após o filtro
  }

  updateFilteredReminders(): void {
    const startIndex = this.pageIndex * this.pageSize;
    this.displayedReminders = this.filteredReminders.slice(startIndex, startIndex + this.pageSize); // Atualiza os lembretes exibidos
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updateFilteredReminders(); // Atualiza os lembretes filtrados quando a página muda
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  createReminder(): void {
    // Abrir um diálogo para criar um novo lembrete
  }

  editReminder(reminder: Reminder): void {
    // Abrir um diálogo para editar o lembrete existente
  }

  viewDetails(reminder: Reminder): void {
    // Abrir um diálogo para visualizar os detalhes do lembrete
  }

  deleteReminder(reminder: Reminder): void {
    // Excluir o lembrete selecionado
  }
}
