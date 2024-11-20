import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Categoria } from '../../../models/categoria.model';

@Component({
  selector: 'app-categoria-detail',
  templateUrl: './categoria-detail.component.html',
  styleUrls: ['./categoria-detail.component.css']
})
export class CategoriaDetailComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: Categoria) {}
}
