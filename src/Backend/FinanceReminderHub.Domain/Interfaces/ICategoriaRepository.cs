using FinanceReminderHub.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<IEnumerable<Categoria>> GetCategorias();
        Task<Categoria> GetCategoriaById(int id);
        Task<bool> CategoriaExiste(int categoriaId);
        Task<int> AddCategoria(Categoria categoria);
        Task UpdateCategoria(Categoria categoria);
        Task DeleteCategoria(int id);
    }
}
