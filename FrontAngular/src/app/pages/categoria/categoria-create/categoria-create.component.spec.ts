import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategoriaCreateComponent } from './categoria-create.component';

describe('CategoriaCreateComponent', () => {
  let component: CategoriaCreateComponent;
  let fixture: ComponentFixture<CategoriaCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategoriaCreateComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategoriaCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
