import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransacaoDetailComponent } from './transacao-detail.component';

describe('TransacaoDetailComponent', () => {
  let component: TransacaoDetailComponent;
  let fixture: ComponentFixture<TransacaoDetailComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacaoDetailComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransacaoDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
