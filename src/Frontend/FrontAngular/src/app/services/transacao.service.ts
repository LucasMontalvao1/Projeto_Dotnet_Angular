import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '@/environments/environment';
import { Transacao } from '../models/transacao.model';
import { TransacaoDto } from '../models/transacao-dto.model';

@Injectable({
  providedIn: 'root'
})
export class TransacaoService {
  private apiUrl = environment.endpoints.transacao;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getTransacoes(): Observable<Transacao[]> {
    return this.http.get<Transacao[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  getTransacaoById(id: number): Observable<Transacao> {
    return this.http.get<Transacao>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }
  
  createTransacao(transacao: TransacaoDto): Observable<Transacao> {
    return this.http.post<Transacao>(this.apiUrl, transacao, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  updateTransacao(transacao: Transacao): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${transacao.transacaoID}`, 
      transacao, 
      { headers: this.getHeaders() }
    ).pipe(catchError(this.handleError));
  }

  deleteTransacao(id: number): Observable<void> {
    return this.http.delete<void>(
      `${this.apiUrl}/${id}`, 
      { headers: this.getHeaders() }
    ).pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('Erro na operação:', error);
    let errorMessage = 'Erro ao realizar a operação.';

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Erro: ${error.error.message}`;
    } else {
      switch (error.status) {
        case 400:
          errorMessage = error.error || 'Dados inválidos.';
          break;
        case 401:
          errorMessage = 'Não autorizado. Por favor, faça login novamente.';
          break;
        case 404:
          errorMessage = 'Transação não encontrada.';
          break;
        case 500:
          errorMessage = 'Erro interno do servidor.';
          break;
        default:
          errorMessage = 'Ocorreu um erro inesperado.';
      }
    }

    return throwError(() => new Error(errorMessage));
  }
}