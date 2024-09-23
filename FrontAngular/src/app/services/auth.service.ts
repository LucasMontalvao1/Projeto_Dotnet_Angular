import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { User } from '../models/User';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.endpoints.login;
  private jwtHelper = new JwtHelperService(); // Instância do JwtHelper para decodificar o token

  constructor(private http: HttpClient) { }

  // Método para realizar o login
  login(loginRequest: LoginRequest): Observable<User> {
    return this.http.post<User>(this.apiUrl, loginRequest).pipe(
      catchError(this.handleError) // Captura erros da API
    );
  }

  // Armazena o token no sessionStorage
  storeToken(token: string): void {
    sessionStorage.setItem('token', token);
    console.log('Token armazenado no sessionStorage:', token); // Log do token armazenado
  }

  // Retorna o token armazenado
  getToken(): string | null {
    const token = sessionStorage.getItem('token');
    console.log('Token recuperado do sessionStorage:', token); // Log do token recuperado
    return token;
  }

  // Verifica se o token ainda é válido (não expirado)
  isAuthenticated(): boolean {
    const token = this.getToken();
    return token ? !this.jwtHelper.isTokenExpired(token) : false;
  }

  // Decodifica e retorna os dados do token
  getDecodedToken(): any {
    const token = this.getToken();
    const decodedToken = token ? this.jwtHelper.decodeToken(token) : null;
    console.log('Dados decodificados do token:', decodedToken); // Log do token decodificado
    return decodedToken;
  }

  // Limpa o token do sessionStorage, efetua logout
  logout(): void {
    sessionStorage.removeItem('token');
    console.log('Token removido do sessionStorage'); // Log do token removido
  }

  // Método para tratar erros da API
  private handleError(error: any): Observable<never> {
    let errorMessage = 'Ocorreu um erro desconhecido!';
    if (error.error instanceof ErrorEvent) {
      // Erro no lado do cliente
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      // Erro no lado do servidor
      errorMessage = `Código do Erro: ${error.status}\nMensagem: ${error.message}`;
    }
    return throwError(errorMessage);
  }
}
