using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Services.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<Transacao>> GetTransacoes();
        Task<Transacao> GetTransacaoById(int id);
        Task AddTransacao(TransacaoDto transacao);
        Task UpdateTransacao(Transacao transacao);
        Task DeleteTransacao(int id);
    }
}
