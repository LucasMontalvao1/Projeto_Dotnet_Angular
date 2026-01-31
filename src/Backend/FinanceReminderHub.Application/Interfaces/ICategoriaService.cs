using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaResponse>> GetCategorias();
        Task<CategoriaResponse> GetCategoriaById(int id);
        Task<int> AddCategoria(CategoriaRequest categoriaRequest);
        Task UpdateCategoria(int id, CategoriaRequest categoriaRequest);
        Task DeleteCategoria(int id);
    }
}
