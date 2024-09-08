using ApiLucas.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySql.Data.MySqlClient;
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

        public User ValidarUsuario(User user)
        {
            User validatedUser = null;

            try
            {
                using (MySqlConnection connection = _mySqlConnectionDB.CreateConnection())
                {
                    string query = "SELECT UsuarioID, Username, Name, Foto, Email, Cargo FROM usuarios WHERE Username = @Username AND Password = @Password";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@Password", user.Password); 

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
                                    Email = reader.GetString("Email"),
                                    Cargo = reader.GetString("Cargo")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your error handling strategy
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return validatedUser; // Return null if the user was not found
        }
    }
}
