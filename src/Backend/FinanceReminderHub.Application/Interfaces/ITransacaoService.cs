using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Application.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoResponse>> GetTransacoes();
        Task<TransacaoResponse> GetTransacaoById(int id);
        Task AddTransacao(TransacaoRequest transacaoRequest);
        Task UpdateTransacao(int id, TransacaoRequest transacaoRequest);
        Task DeleteTransacao(int id);
    }
}
