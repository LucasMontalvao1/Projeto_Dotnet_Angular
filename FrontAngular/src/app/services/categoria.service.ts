// services/categoria.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '@/environments/environment';
import { Categoria } from '../models/categoria.model';

@Injectable({
  providedIn: 'root'
})
export class CategoriaService {
  private apiUrl = environment.endpoints.categoria;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  getCategoriaById(id: number): Observable<Categoria> {
    return this.http.get<Categoria>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }
  
  createCategoria(categoria: Categoria): Observable<Categoria> {
    return this.http.post<Categoria>(this.apiUrl, categoria, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  updateCategoria(categoria: Categoria): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${categoria.categoriaID}`, 
      categoria, 
      { headers: this.getHeaders() }
    ).pipe(catchError(this.handleError));
  }

  deleteCategoria(id: number): Observable<void> {
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
          errorMessage = 'Categoria não encontrada.';
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