import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LembretesComponent } from './lembretes.component';

describe('LembretesComponent', () => {
  let component: LembretesComponent;
  let fixture: ComponentFixture<LembretesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LembretesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LembretesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
