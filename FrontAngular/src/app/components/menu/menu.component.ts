// menu.component.ts
import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent {
  isScrolled = false;
  userName: string = 'Usuário'; 

  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    // Detecta scroll para adicionar sombra no menu
    window.addEventListener('scroll', () => {
      this.isScrolled = window.scrollY > 10;
    });
  }

  logout(): void {
    this.snackBar.open('Logout realizado com sucesso!', 'Fechar', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}