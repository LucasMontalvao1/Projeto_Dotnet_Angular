import { Categoria } from "./categoria.model";

export interface Transacao {
    transacaoID: number;
    usuarioID: number;
    categoriaID: number;  
    categoria: Categoria; 
    valor: number;
    descricao: string;
    data: Date;
    criadoEm: Date;
    tipo: string;
}