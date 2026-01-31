import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LembretesExcluirComponent } from './lembretes-excluir.component';

describe('LembretesExcluirComponent', () => {
  let component: LembretesExcluirComponent;
  let fixture: ComponentFixture<LembretesExcluirComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LembretesExcluirComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LembretesExcluirComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
