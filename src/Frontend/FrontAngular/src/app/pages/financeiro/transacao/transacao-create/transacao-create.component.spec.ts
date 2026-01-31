import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransacaoCreateComponent } from './transacao-create.component';

describe('TransacaoCreateComponent', () => {
  let component: TransacaoCreateComponent;
  let fixture: ComponentFixture<TransacaoCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacaoCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransacaoCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
