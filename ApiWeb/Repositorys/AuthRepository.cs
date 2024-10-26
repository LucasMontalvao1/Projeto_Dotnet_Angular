using ApiLucas.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySqlConnector;
using System;

namespace ApiWeb.Repositorys
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MySqlConnectionDB _mySqlConnectionDB;

        public AuthRepository(MySqlConnectionDB mySqlConnectionDB)
        {
            _mySqlConnectionDB = mySqlConnectionDB;
        }

        public User ValidarUsuario(string username, string password)
        {
            User validatedUser = null;

            try
            {
                using (MySqlConnection connection = _mySqlConnectionDB.CreateConnection())
                {
                    string query = "SELECT UsuarioID, Username, Name, Foto, Email FROM usuarios WHERE Username = @Username AND Password = @Password";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                validatedUser = new User
                                {
                                    UsuarioID = reader.GetInt32("UsuarioID"),
                                    Username = reader.GetString("Username"),
                                    Name = reader.GetString("Name"),
                                    Foto = reader.GetString("Foto"),
                                    Email = reader.GetString("Email")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return validatedUser;
        }

        public async Task<bool> UsuarioExiste(int usuarioId)
        {
            try
            {
                using (MySqlConnection connection = _mySqlConnectionDB.CreateConnection())
                {
                    string query = "SELECT COUNT(1) FROM usuarios WHERE UsuarioID = @UsuarioID";

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
                Console.WriteLine($"An error occurred while checking user existence: {ex.Message}");
                return false;
            }

        }
    }
}
