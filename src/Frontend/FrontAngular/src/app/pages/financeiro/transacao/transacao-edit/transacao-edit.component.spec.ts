import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransacaoEditComponent } from './transacao-edit.component';

describe('TransacaoEditComponent', () => {
  let component: TransacaoEditComponent;
  let fixture: ComponentFixture<TransacaoEditComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransacaoEditComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransacaoEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
