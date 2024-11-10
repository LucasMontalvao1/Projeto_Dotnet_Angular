import { Categoria } from "./categoria.model";

export interface Transacao {
    transacaoID: number;
    usuarioID: number;
    valor: number;
    descricao: string;
    data: Date;
    tipo: string;
    categoriaID: string;
}