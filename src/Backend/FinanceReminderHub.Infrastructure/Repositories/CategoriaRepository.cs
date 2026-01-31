using Dapper;
using FinanceReminderHub.Domain.Entities;
using FinanceReminderHub.Domain.Interfaces;
using FinanceReminderHub.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly DapperContext _context;

        public CategoriaRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            var query = "SELECT CategoriaID, Nome, Descricao FROM categorias";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Categoria>(query);
            }
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            var query = "SELECT CategoriaID, Nome, Descricao FROM categorias WHERE CategoriaID = @Id";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Categoria>(query, new { Id = id });
            }
        }

        public async Task<bool> CategoriaExiste(int categoriaId)
        {
            var query = "SELECT COUNT(1) FROM categorias WHERE CategoriaID = @Id";
            using (var connection = _context.CreateConnection())
            {
                var count = await connection.ExecuteScalarAsync<int>(query, new { Id = categoriaId });
                return count > 0;
            }
        }

        public async Task<int> AddCategoria(Categoria categoria)
        {
            var query = @"INSERT INTO categorias (Nome, Descricao) VALUES (@Nome, @Descricao);
                          SELECT LAST_INSERT_ID();";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, categoria);
            }
        }

        public async Task UpdateCategoria(Categoria categoria)
        {
            var query = "UPDATE categorias SET Nome = @Nome, Descricao = @Descricao WHERE CategoriaID = @CategoriaID";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, categoria);
            }
        }

        public async Task DeleteCategoria(int id)
        {
            var query = "DELETE FROM categorias WHERE CategoriaID = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
