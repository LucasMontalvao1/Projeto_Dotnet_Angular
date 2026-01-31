using Dapper;
using FinanceReminderHub.Domain.Entities;
using FinanceReminderHub.Domain.Interfaces;
using FinanceReminderHub.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly DapperContext _context;

        public TransacaoRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transacao>> GetTransacoes()
        {
            // Dapper's Multi-Mapping to Map Transacao + Categoria
            var query = @"SELECT t.*, c.*
                          FROM Transacoes t
                          INNER JOIN Categorias c ON t.CategoriaID = c.CategoriaID";

            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Transacao, Categoria, Transacao>(
                    query,
                    (transacao, categoria) =>
                    {
                        transacao.Categoria = categoria;
                        return transacao;
                    },
                    splitOn: "CategoriaID");
            }
        }

        public async Task<Transacao> GetTransacaoById(int id)
        {
            var query = @"SELECT t.*, c.*
                          FROM Transacoes t
                          INNER JOIN Categorias c ON t.CategoriaID = c.CategoriaID
                          WHERE t.TransacaoID = @Id";

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Transacao, Categoria, Transacao>(
                    query,
                    (transacao, categoria) =>
                    {
                        transacao.Categoria = categoria;
                        return transacao;
                    },
                    new { Id = id },
                    splitOn: "CategoriaID");

                return System.Linq.Enumerable.FirstOrDefault(result);
            }
        }

        public async Task AddTransacao(Transacao transacao)
        {
            var query = @"INSERT INTO Transacoes (UsuarioID, CategoriaID, Tipo, Valor, Descricao, Data, CriadoEm) 
                          VALUES (@UsuarioID, @CategoriaID, @Tipo, @Valor, @Descricao, @Data, @CriadoEm);
                          SELECT LAST_INSERT_ID();";

            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, transacao);
                transacao.TransacaoID = id;
            }
        }

        public async Task UpdateTransacao(Transacao transacao)
        {
            var query = @"UPDATE Transacoes 
                          SET UsuarioID = @UsuarioID, 
                              CategoriaID = @CategoriaID, 
                              Tipo = @Tipo, 
                              Valor = @Valor, 
                              Descricao = @Descricao, 
                              Data = @Data 
                          WHERE TransacaoID = @TransacaoID";

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, transacao);
            }
        }

        public async Task DeleteTransacao(int id)
        {
            var query = "DELETE FROM Transacoes WHERE TransacaoID = @Id";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
