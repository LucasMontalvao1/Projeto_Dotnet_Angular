using MySql.Data.MySqlClient;

namespace ApiLucas.Infra.Data
{
    public class MySqlConnectionDB
    {
        private readonly string _connectionString;

        public MySqlConnectionDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConfiguracaoPadrao");
        }

        public MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
