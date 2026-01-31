using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Application.Interfaces
{
    public interface ILembreteService
    {
        Task<IEnumerable<LembreteResponse>> GetLembretesByUsuarioId(int usuarioId);
        Task<LembreteResponse> GetLembreteById(int id);
        Task<int> AddLembrete(LembreteRequest lembreteRequest);
        Task UpdateLembrete(int id, LembreteRequest lembreteRequest);
        Task DeleteLembrete(int id);
    }
}
