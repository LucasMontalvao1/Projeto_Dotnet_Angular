import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Transacao } from '../../../../models/transacao.model';

@Component({
  selector: 'app-transacao-detail',
  templateUrl: './transacao-detail.component.html',
  styleUrls: ['./transacao-detail.component.css']
})
export class TransacaoDetailComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: Transacao) {}
}