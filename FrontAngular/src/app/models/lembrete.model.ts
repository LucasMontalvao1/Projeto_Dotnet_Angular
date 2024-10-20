export interface Lembrete {
  lembreteID?: number;
  usuarioID: number;
  titulo: string;
  descricao: string;
  dataLembrete: Date;
  intervaloEmDias: number;
  criadoEm?: Date;

}
