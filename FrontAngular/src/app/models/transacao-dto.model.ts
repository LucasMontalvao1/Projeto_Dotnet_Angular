import { Categoria } from "./categoria.model";

export interface TransacaoDto {
    usuarioID: number;
    valor: number;
    descricao: string;
    data: Date;
    tipo: string;
    categoriaID: Categoria;
}