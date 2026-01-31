using FinanceReminderHub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Domain.Interfaces
{
    public interface ILembreteRepository
    {
        Task<IEnumerable<Lembrete>> GetLembretesByUsuarioId(int usuarioId);
        Task<IEnumerable<Lembrete>> GetAllLembretes();
        Task<Lembrete> GetLembreteById(int lembreteId);
        Task<int> AddLembrete(Lembrete lembrete);
        Task UpdateLembrete(Lembrete lembrete);
        Task<bool> DeleteLembrete(int lembreteId);
    }
}
