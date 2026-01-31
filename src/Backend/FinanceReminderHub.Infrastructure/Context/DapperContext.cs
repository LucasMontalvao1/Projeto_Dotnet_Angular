using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data;

namespace FinanceReminderHub.Infrastructure.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("ConfiguracaoPadrao");
        }

        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
}
