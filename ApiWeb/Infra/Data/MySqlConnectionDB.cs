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
            try
            {
                return new MySqlConnection(_connectionString);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Erro ao criar conexão com o banco de dados.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Um erro inesperado ocorreu ao criar a conexão.", ex);
            }
        }
    }
}
