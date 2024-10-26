using ApiWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<Categoria>> GetCategorias();
        Task<Categoria> GetCategoriaById(int id);
        Task<int> AddCategoria(Categoria categoria);
        Task UpdateCategoria(Categoria categoria);
        Task DeleteCategoria(int id);
    }
}
