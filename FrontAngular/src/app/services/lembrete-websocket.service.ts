import { Injectable } from '@angular/core';
import { RxStomp } from '@stomp/rx-stomp';
import { Subject, Observable } from 'rxjs';
import { Lembrete } from '../models/lembrete.model';

@Injectable({
  providedIn: 'root'
})
export class LembreteWebSocketService {
  private rxStomp: RxStomp;
  private lembreteSubject = new Subject<Lembrete>();

  constructor() {
    this.rxStomp = new RxStomp();
    this.configureRxStomp();
    this.initAndConnect();
  }

  private configureRxStomp(): void {
    this.rxStomp.configure({
      brokerURL: 'ws://localhost:15674/ws',
      connectHeaders: {},
      heartbeatIncoming: 0,
      heartbeatOutgoing: 20000,
      reconnectDelay: 5000,
      debug: (str: string) => { console.log(new Date(), str); }
    });
  }

  private initAndConnect(): void {
    this.rxStomp.activate(); // Ativa a conexão WebSocket

    this.rxStomp.connected$.subscribe(() => {
      console.log('Conectado ao WebSocket!');
      this.subscribeToLembretes(); // Inscreve-se para receber lembretes
    }, (error) => {
      console.error('Erro ao conectar ao WebSocket:', error);
    });
  }

  private subscribeToLembretes(): void {
    this.rxStomp.watch('/queue/filaLembretes').subscribe({
      next: (message) => {
        console.log('Mensagem recebida do servidor:', message);
        if (message.body) {
          try {
            const lembrete: Lembrete = JSON.parse(message.body);
            console.log('Novo lembrete recebido:', lembrete);
            this.lembreteSubject.next(lembrete);
          } catch (error) {
            console.error('Erro ao fazer parse do lembrete:', error);
          }
        }
      },
      error: (error) => {
        console.error('Erro ao receber mensagem:', error);
      }
    });
  }

  getLembretes(): Observable<Lembrete> {
    return this.lembreteSubject.asObservable();
  }

  disconnect(): void {
    this.rxStomp.deactivate();
    console.log('Desconectado do WebSocket.');
  }
}
