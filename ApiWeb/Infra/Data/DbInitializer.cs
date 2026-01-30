using MySqlConnector;
using System.Data;

namespace ApiWeb.Infra.Data
{
    public static class DbInitializer
    {
        public static void Initialize(string connectionString, string scriptsBaseBoundPath)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;
            
            // Conexão sem o banco de dados para criá-lo se necessário
            builder.Database = "";
            var masterConnectionString = builder.ConnectionString;

            using (var connection = new MySqlConnection(masterConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}`;";
                    command.ExecuteNonQuery();
                }
            }

            // Agora conecta ao banco criado para rodar os scripts
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // 1. Tabelas
                ExecuteScriptsFromDirectory(connection, Path.Combine(scriptsBaseBoundPath, "01_tables"));

                // 2. Procedures
                ExecuteScriptsFromDirectory(connection, Path.Combine(scriptsBaseBoundPath, "02_procedures"));

                // 3. Triggers
                ExecuteScriptsFromDirectory(connection, Path.Combine(scriptsBaseBoundPath, "03_triggers"));

                // 4. Seeds (opcional, apenas se tabelas estiverem vazias)
                if (IsTableEmpty(connection, "Categorias"))
                {
                    ExecuteScriptsFromDirectory(connection, Path.Combine(scriptsBaseBoundPath, "04_seeds"));
                }
            }
        }

        private static void ExecuteScriptsFromDirectory(MySqlConnection connection, string path)
        {
            if (!Directory.Exists(path)) return;

            var files = Directory.GetFiles(path, "*.sql").OrderBy(f => f);
            foreach (var file in files)
            {
                var script = File.ReadAllText(file);
                ExecuteMultiCommandScript(connection, script);
            }
        }

        private static void ExecuteMultiCommandScript(MySqlConnection connection, string script)
        {
            // Tratamento simples para scripts com DELIMITER
            var commands = script.Split(new[] { "DELIMITER //", "DELIMITER ;", "//" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var cmdText in commands)
            {
                var trimmedCmd = cmdText.Trim();
                if (string.IsNullOrWhiteSpace(trimmedCmd)) continue;

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = trimmedCmd;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        // Log ou ignorar se o erro for de objeto já existente
                        if (ex.Number != 1050 && ex.Number != 1061 && ex.Number != 1062) 
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private static bool IsTableEmpty(MySqlConnection connection, string tableName)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(1) FROM `{tableName}`";
                try
                {
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count == 0;
                }
                catch
                {
                    return true;
                }
            }
        }
    }
}
