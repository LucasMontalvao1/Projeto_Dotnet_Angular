using ApiWeb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Repositorys.Interfaces
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
