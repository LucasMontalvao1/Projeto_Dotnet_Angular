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
      connectHeaders: {
        login: 'guest',  
        passcode: 'guest'  
      },
      heartbeatIncoming: 4000,
      heartbeatOutgoing: 4000,
      reconnectDelay: 5000,
      debug: (str: string) => { console.log(new Date(), str); },
      // Configurações para mensagens binárias
      forceBinaryWSFrames: true,
      appendMissingNULLonIncoming: true
    });
  }

  private initAndConnect(): void {
    console.log('Iniciando conexão WebSocket...');
    this.rxStomp.activate();

    this.rxStomp.connected$.subscribe({
      next: () => {
        console.log('Conectado ao WebSocket!');
        this.subscribeToLembretes();
      },
      error: (error) => {
        console.error('Erro ao conectar ao WebSocket:', error);
      }
    });
  }

  private subscribeToLembretes(): void {
    this.rxStomp.watch('/queue/filaLembretes').subscribe({
      next: (message) => {
        console.log('Tipo da mensagem:', typeof message.body);
        console.log('É binário?', message.isBinaryBody);
        console.log('Headers:', message.headers);

        try {
          let conteudo: string;

          if (message.isBinaryBody && message.binaryBody) {
            // Converte o conteúdo binário para string
            conteudo = new TextDecoder().decode(message.binaryBody);
            console.log('Conteúdo decodificado:', conteudo);
          } else {
            conteudo = message.body;
          }

          // Tenta fazer o parse do JSON
          if (conteudo) {
            try {
              const lembrete: Lembrete = JSON.parse(conteudo);
              console.log('Lembrete processado:', lembrete);
              this.lembreteSubject.next(lembrete);
            } catch (parseError) {
              // Se não for JSON válido, tenta criar um objeto com o texto recebido
              console.log('Recebido texto simples:', conteudo);
              const lembreteSimples: Lembrete = {
                descricao: conteudo,
                // Adicione outros campos necessários com valores padrão
                dataLembrete: new Date(),
                usuarioID: 0,
                titulo: '',
                intervaloEmDias: 0
              };
              this.lembreteSubject.next(lembreteSimples);
            }
          }
        } catch (error) {
          console.error('Erro ao processar mensagem:', error);
          console.error('Conteúdo da mensagem que causou erro:', message);
        }
      },
      error: (error) => {
        console.error('Erro na subscrição:', error);
      }
    });
  }

  getLembretes(): Observable<Lembrete> {
    return this.lembreteSubject.asObservable();
  }

  disconnect(): void {
    try {
      this.rxStomp.deactivate();
      console.log('Desconectado do WebSocket.');
    } catch (error) {
      console.error('Erro ao desconectar:', error);
    }
  }

  // Método auxiliar para reconexão
  reconnect(): void {
    console.log('Tentando reconectar...');
    this.disconnect();
    setTimeout(() => {
      this.configureRxStomp();
      this.initAndConnect();
    }, 2000);
  }

  // Método para verificar status da conexão
  isConnected(): boolean {
    return this.rxStomp.connected();
  }
}