import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransacaoTotaisComponent } from './transacao-totais.component';

describe('TransacaoTotaisComponent', () => {
  let component: TransacaoTotaisComponent;
  let fixture: ComponentFixture<TransacaoTotaisComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacaoTotaisComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransacaoTotaisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
