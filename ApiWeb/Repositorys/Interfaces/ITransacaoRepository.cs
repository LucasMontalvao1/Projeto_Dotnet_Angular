using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> GetTransacoes();
        Task<Transacao> GetTransacaoById(int id);
        Task AddTransacao(TransacaoDto transacaoDto);
        Task UpdateTransacao(Transacao transacao);
        Task DeleteTransacao(int id);
    }
}
