import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = null;
      const loginRequest = this.loginForm.value;
      console.log('Requisição de login:', loginRequest); // Log da requisição

      this.authService.login(loginRequest).subscribe({
        next: (user) => {
          console.log('Resposta do login:', user);
          if (user && user.token) {
            this.authService.storeToken(user.token);
            this.router.navigate(['/home']);
            this.snackBar.open('Login realizado com sucesso!', '', { duration: 3000 });
          } else {
            this.errorMessage = 'Token não recebido. Login falhou.';
            this.snackBar.open(this.errorMessage, '', { duration: 3000 });
          }
        },
        error: (error) => {
          this.errorMessage = 'Erro ao fazer login: ' + error; // Armazena mensagem de erro
          this.snackBar.open(this.errorMessage, '', { duration: 3000 });
          this.isLoading = false;
        },
        complete: () => {
          this.isLoading = false;
        }
      });
    }
  }
}
