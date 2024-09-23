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
  isLoading = false; // Controla o estado do botão

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
      this.isLoading = true; // Inicia o estado de carregamento
      this.authService.login(this.loginForm.value).subscribe({
        next: (user) => {
          this.authService.storeToken(user.token);
          this.router.navigate(['/home']);
          this.snackBar.open('Login realizado com sucesso!', '', { duration: 3000 });
        },
        error: (error) => {
          this.snackBar.open('Erro ao fazer login: ' + error, '', { duration: 3000 });
        },
        complete: () => {
          this.isLoading = false; // Finaliza o estado de carregamento
        }
      });
    }
  }
}
