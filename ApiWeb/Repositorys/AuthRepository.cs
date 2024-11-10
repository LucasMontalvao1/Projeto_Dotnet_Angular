using ApiWeb.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySqlConnector;
using System;
using System.Data;

namespace ApiWeb.Repositorys
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MySqlConnectionDB _mySqlConnectionDB;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(MySqlConnectionDB mySqlConnectionDB, ILogger<AuthRepository> logger)
        {
            _mySqlConnectionDB = mySqlConnectionDB;
            _logger = logger;
        }

        public User ValidarUsuario(string username, string password)
        {
            User validatedUser = null;
            try
            {
                using (MySqlConnection connection = _mySqlConnectionDB.CreateConnection())
                {
                    string query = @"SELECT UsuarioID, Username, Name, Foto, Email 
                                   FROM usuario 
                                   WHERE Username = @Username 
                                   AND Password = @Password";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        _logger.LogInformation("Tentando validar usuário: {Username}", username);

                        connection.Open();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                validatedUser = new User
                                {
                                    UsuarioID = reader.GetInt32("UsuarioID"),
                                    Username = !reader.IsDBNull("Username") ? reader.GetString("Username") : string.Empty,
                                    Name = !reader.IsDBNull("Name") ? reader.GetString("Name") : string.Empty,
                                    Foto = !reader.IsDBNull("Foto") ? reader.GetString("Foto") : string.Empty,
                                    Email = !reader.IsDBNull("Email") ? reader.GetString("Email") : string.Empty
                                };

                                _logger.LogInformation("Usuário encontrado: {Username}", validatedUser.Username);
                            }
                            else
                            {
                                _logger.LogWarning("Nenhum usuário encontrado para o username: {Username}", username);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar usuário: {Username}", username);
                throw; 
            }

            return validatedUser;
        }

        public async Task<bool> UsuarioExiste(int usuarioId)
        {
            try
            {
                using (MySqlConnection connection = _mySqlConnectionDB.CreateConnection())
                {
                    string query = "SELECT COUNT(1) FROM usuario WHERE UsuarioID = @UsuarioID";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioID", usuarioId);
                        await connection.OpenAsync();

                        var exists = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(exists) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do usuário: {UsuarioId}", usuarioId);
                throw; 
            }
        }
    }
}