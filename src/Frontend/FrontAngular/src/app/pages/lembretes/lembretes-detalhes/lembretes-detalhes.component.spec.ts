import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LembretesDetalhesComponent } from './lembretes-detalhes.component';

describe('LembretesDetalhesComponent', () => {
  let component: LembretesDetalhesComponent;
  let fixture: ComponentFixture<LembretesDetalhesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LembretesDetalhesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LembretesDetalhesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
