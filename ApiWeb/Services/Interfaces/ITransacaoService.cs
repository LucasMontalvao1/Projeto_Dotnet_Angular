using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Services.Interfaces
{
    public interface ITransacaoService
    {
        /// <summary>
        /// Retorna todas as transações com suas respectivas categorias
        /// </summary>
        /// <returns>Lista de transações com dados completos das categorias</returns>
        Task<IEnumerable<Transacao>> GetTransacoes();

        /// <summary>
        /// Busca uma transação específica por ID com sua categoria
        /// </summary>
        /// <param name="id">ID da transação</param>
        /// <returns>Transação com dados completos da categoria</returns>
        Task<Transacao> GetTransacaoById(int id);

        /// <summary>
        /// Adiciona uma nova transação
        /// </summary>
        /// <param name="transacao">DTO da transação com objeto Categoria completo</param>
        Task AddTransacao(TransacaoDto transacao);

        /// <summary>
        /// Atualiza uma transação existente
        /// </summary>
        /// <param name="transacao">Dados atualizados da transação</param>
        Task UpdateTransacao(TransacaoDto transacao);

        /// <summary>
        /// Remove uma transação
        /// </summary>
        /// <param name="id">ID da transação a ser removida</param>
        Task DeleteTransacao(int id);
    }
}