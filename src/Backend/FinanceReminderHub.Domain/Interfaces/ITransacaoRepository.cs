using FinanceReminderHub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Domain.Interfaces
{
    public interface ITransacaoRepository
    {
        Task<IEnumerable<Transacao>> GetTransacoes();
        Task<Transacao> GetTransacaoById(int id);
        Task AddTransacao(Transacao transacao); // Changed from TransacaoDto to Transacao Entity
        Task UpdateTransacao(Transacao transacao);
        Task DeleteTransacao(int id);
    }
}
