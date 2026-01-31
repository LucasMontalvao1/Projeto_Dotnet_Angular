using Dapper;
using FinanceReminderHub.Domain.Entities;
using FinanceReminderHub.Domain.Interfaces;
using FinanceReminderHub.Infrastructure.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Infrastructure.Repositories
{
    public class LembreteRepository : ILembreteRepository
    {
        private readonly DapperContext _context;

        public LembreteRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Lembrete>> GetLembretesByUsuarioId(int usuarioId)
        {
            var query = "SELECT * FROM lembretes WHERE UsuarioID = @UsuarioID";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Lembrete>(query, new { UsuarioID = usuarioId });
            }
        }

        public async Task<IEnumerable<Lembrete>> GetAllLembretes()
        {
            var query = "SELECT * FROM lembretes";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Lembrete>(query);
            }
        }

        public async Task<Lembrete> GetLembreteById(int lembreteId)
        {
            var query = "SELECT * FROM lembretes WHERE LembreteID = @LembreteID";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Lembrete>(query, new { LembreteID = lembreteId });
            }
        }

        public async Task<int> AddLembrete(Lembrete lembrete)
        {
            var query = @"INSERT INTO lembretes (UsuarioID, Titulo, Descricao, DataLembrete, IntervaloEmDias, CriadoEm) 
                          VALUES (@UsuarioID, @Titulo, @Descricao, @DataLembrete, @IntervaloEmDias, @CriadoEm);
                          SELECT LAST_INSERT_ID();";
            using (var connection = _context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, lembrete);
                lembrete.LembreteID = id;
                return id;
            }
        }

        public async Task UpdateLembrete(Lembrete lembrete)
        {
            var query = @"UPDATE lembretes 
                          SET Titulo = @Titulo, Descricao = @Descricao, DataLembrete = @DataLembrete, IntervaloEmDias = @IntervaloEmDias 
                          WHERE LembreteID = @LembreteID";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, lembrete);
            }
        }

        public async Task<bool> DeleteLembrete(int lembreteId)
        {
            var query = "DELETE FROM lembretes WHERE LembreteID = @LembreteID";
            using (var connection = _context.CreateConnection())
            {
                var rows = await connection.ExecuteAsync(query, new { LembreteID = lembreteId });
                return rows > 0;
            }
        }
    }
}
