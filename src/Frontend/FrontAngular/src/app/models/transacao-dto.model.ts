export interface TransacaoDto {
  usuarioID: number;
  categoriaID: number;
  tipo: string;
  valor: number;
  descricao: string;
  data: Date | string;
  criadoEm: Date | string;
  transacaoID?: number | null;
}