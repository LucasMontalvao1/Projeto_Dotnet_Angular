export interface TransacaoDto {
    usuarioID: number;
    categoriaID: number;
    tipo: string;
    valor: number;
    descricao: string;
    data: Date;
    criadoEm: Date;
  }