import { TestBed } from '@angular/core/testing';

import { LembreteWebsocketService } from './lembrete-websocket.service';

describe('LembreteWebsocketService', () => {
  let service: LembreteWebsocketService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LembreteWebsocketService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
