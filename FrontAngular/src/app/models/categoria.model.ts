export interface Categoria {
    categoriaID: number;
    nome: string;
    descricao?: string;
    ativo: boolean;
    usuarioID: number;
}