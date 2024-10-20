import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Lembrete } from '../models/lembrete.model';
import { environment } from '@/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class LembreteService {
  private apiUrl = environment.endpoints.lembretes;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = sessionStorage.getItem('token');
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }

  getLembretes(): Observable<Lembrete[]> {
    return this.http.get<Lembrete[]>(`${this.apiUrl}/todos`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  getLembreteById(id: number): Observable<Lembrete> {
    return this.http.get<Lembrete>(`${this.apiUrl}/${id}`);
  }
  
  createLembrete(lembrete: Lembrete): Observable<Lembrete> {
    return this.http.post<Lembrete>(this.apiUrl, lembrete, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  editLembrete(lembrete: Lembrete): Observable<Lembrete> {
    return this.http.put<Lembrete>(`${this.apiUrl}/${lembrete.lembreteID}`, lembrete, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  deleteLembrete(lembreteId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${lembreteId}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('Ocorreu um erro:', error);
    return throwError(() => new Error('Erro ao realizar a operação.'));
  }
}
