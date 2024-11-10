import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransacaoDeleteComponent } from './transacao-delete.component';

describe('TransacaoDeleteComponent', () => {
  let component: TransacaoDeleteComponent;
  let fixture: ComponentFixture<TransacaoDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacaoDeleteComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransacaoDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
