import { Component, OnInit  } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;  
  isLoading = false;
  errorMessage: string | null = null;
  hidePassword = true;
  year = new Date().getFullYear();

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(1)]],
      password: ['', [Validators.required, Validators.minLength(1)]],
      rememberMe: [false]
    });
  }

  ngOnInit(): void {
    
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = null;
      const { username, password, rememberMe } = this.loginForm.value;
      
      this.authService.login({ username, password }).subscribe({
        next: (user) => {
          if (user && user.token) {
            if (rememberMe) {
              localStorage.setItem('rememberMe', 'true');
            }
            this.authService.storeToken(user.token);
            this.router.navigate(['/home']);
            this.showSuccessMessage('Bem-vindo de volta!');
          } else {
            this.handleError('Falha na autenticação');
          }
        },
        error: (error) => {
          this.handleError(error);
        }
      });
    } else {
      this.markFormGroupTouched(this.loginForm);
    }
  }

  private showSuccessMessage(message: string): void {
    this.snackBar.open(message, 'Fechar', {
      duration: 3000,
      panelClass: ['success-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
  }

  private handleError(error: any): void {
    const errorMessage = error?.error?.message || 'Acesso negado, verifique o login e senha!';
    this.errorMessage = errorMessage;
    this.snackBar.open(errorMessage, 'Fechar', {
      duration: 5000,
      panelClass: ['error-snackbar'],
      horizontalPosition: 'end',
      verticalPosition: 'top'
    });
    this.isLoading = false;
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  getErrorMessage(controlName: string): string {
    const control = this.loginForm.get(controlName);
    if (control?.hasError('required')) {
      return `${controlName === 'username' ? 'Usuário' : 'Senha'} é obrigatório`;
    }
    if (control?.hasError('minlength')) {
      return `${controlName === 'username' ? 'Usuário' : 'Senha'} deve ter no mínimo ${
        control.errors?.['minlength'].requiredLength
      } caracteres`;
    }
    return '';
  }
}