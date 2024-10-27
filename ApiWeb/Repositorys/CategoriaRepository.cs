using ApiWeb.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApiWeb.Repositorys
{
    public class CategoriaRepository : ICategoriaRepository, IDisposable
    {
        private readonly MySqlConnectionDB _mySqlConnectionDB;
        private MySqlConnection _connection;

        public CategoriaRepository(MySqlConnectionDB mySqlConnectionDB)
        {
            _mySqlConnectionDB = mySqlConnectionDB;
            _connection = mySqlConnectionDB.CreateConnection();
        }

        public async Task<IEnumerable<Categoria>> GetCategorias()
        {
            List<Categoria> categorias = new List<Categoria>();
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT CategoriaID, Nome, Descricao FROM categorias";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Categoria categoria = new Categoria
                            {
                                CategoriaID = reader.GetInt32("CategoriaID"),
                                Nome = reader.GetString("Nome"),
                                Descricao = reader.IsDBNull("Descricao") ? null : reader.GetString("Descricao")
                            };

                            categorias.Add(categoria);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return categorias;
        }

        public async Task<Categoria> GetCategoriaById(int id)
        {
            Categoria categoria = null;
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT CategoriaID, Nome, Descricao FROM categorias WHERE CategoriaID = @CategoriaID";
                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@CategoriaID", id);

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            categoria = new Categoria
                            {
                                CategoriaID = reader.GetInt32("CategoriaID"),
                                Nome = reader.GetString("Nome"),
                                Descricao = reader.IsDBNull("Descricao") ? null : reader.GetString("Descricao")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return categoria;
        }

        public async Task<bool> CategoriaExiste(int categoriaId)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = "SELECT COUNT(1) FROM categorias WHERE CategoriaID = @CategoriaID";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@CategoriaID", categoriaId);

                    var exists = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(exists) > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while checking category existence: {ex.Message}");
                return false;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<int> AddCategoria(Categoria categoria)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"INSERT INTO categorias (Nome, Descricao) VALUES (@Nome, @Descricao);
                         SELECT LAST_INSERT_ID();"; // Para obter o ID gerado

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Nome", categoria.Nome);
                    command.Parameters.AddWithValue("@Descricao", categoria.Descricao ?? (object)DBNull.Value);

                    // Execute a inserção e obtenha o ID gerado
                    var idGerado = Convert.ToInt32(await command.ExecuteScalarAsync());
                    return idGerado; // Retorna o ID gerado
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                // Você pode decidir lançar a exceção novamente ou retornar um valor padrão
                throw new Exception($"Erro ao adicionar categoria: {ex.Message}"); // Para tratamento externo
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task UpdateCategoria(Categoria categoria)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"UPDATE categorias SET Nome = @Nome, Descricao = @Descricao WHERE CategoriaID = @CategoriaID";
                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@Nome", categoria.Nome);
                    command.Parameters.AddWithValue("@Descricao", categoria.Descricao ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CategoriaID", categoria.CategoriaID);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task DeleteCategoria(int id)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"DELETE FROM categorias WHERE CategoriaID = @CategoriaID";
                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@CategoriaID", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
