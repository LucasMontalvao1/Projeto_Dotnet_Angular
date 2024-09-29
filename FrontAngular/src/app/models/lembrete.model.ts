export interface Lembrete {
  id: number;
  usuarioID: number;    // Referência ao usuário, conforme o campo UsuarioID no banco
  titulo: string;       // Referência ao campo Titulo
  descricao: string;    // Referência ao campo Descricao
  dataLembrete: Date;   // Referência ao campo DataLembrete, utilizando o tipo Date
  criadoEm?: Date;      // Referência ao campo CriadoEm, pode ser opcional e utilizar Date
}
