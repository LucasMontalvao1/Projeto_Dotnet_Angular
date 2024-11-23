using Xunit;
using ApiWeb.Infra.Data;
using MySqlConnector;
using System;

namespace ApiWeb.Tests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        private readonly MySqlConnection _connection;

        public DatabaseFixture()
        {
            var connectionString = "server=localhost;userid=root;password=asd123;database=projeto_producao";
            _connection = new MySqlConnection(connectionString);
            _connection.Open();

            // Aqui você pode configurar o seu banco de dados, como criar tabelas ou inserir dados de teste
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}