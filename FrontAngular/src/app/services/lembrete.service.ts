import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Lembrete } from '../models/lembrete.model';

@Injectable({
  providedIn: 'root'
})
export class LembreteService {
  private apiUrl = 'https://localhost:7103/api/v1/lembretes';

  constructor(private http: HttpClient) { }


  getLembretes(): Observable<Lembrete[]> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.get<Lembrete[]>(`${this.apiUrl}/todos`, { headers });
  }
}
