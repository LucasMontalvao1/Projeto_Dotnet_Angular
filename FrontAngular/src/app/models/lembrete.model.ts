export interface Lembrete {
  id: number;
  usuarioID: number;
  titulo: string;
  descricao: string;
  dataLembrete: Date;
  criadoEm?: Date;
}
