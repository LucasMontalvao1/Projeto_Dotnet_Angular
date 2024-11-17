using ApiWeb.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Repositorys.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApiWeb.Repositorys
{
    public class TransacaoRepository : ITransacaoRepository, IDisposable
    {
        private readonly MySqlConnection _connection;
        private readonly IAuthRepository _authRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TransacaoRepository(MySqlConnectionDB mySqlConnectionDB, IAuthRepository authRepository, ICategoriaRepository categoriaRepository)
        {
            _connection = mySqlConnectionDB.CreateConnection();
            _authRepository = authRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IEnumerable<Transacao>> GetTransacoes()
        {
            List<Transacao> transacoes = new List<Transacao>();

            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT t.TransacaoID, t.UsuarioID, t.CategoriaID, t.Tipo, t.Valor, 
                              t.Descricao, t.Data, t.CriadoEm, c.Nome as CategoriaNome
                              FROM Transacoes t
                              INNER JOIN Categorias c ON t.CategoriaID = c.CategoriaID";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Transacao transacao = new Transacao
                            {
                                TransacaoID = reader.GetInt32("TransacaoID"),
                                UsuarioID = reader.GetInt32("UsuarioID"),
                                CategoriaID = reader.GetInt32("CategoriaID"),
                                Tipo = reader.GetString("Tipo"),
                                Valor = reader.GetDecimal("Valor"),
                                Descricao = reader.IsDBNull("Descricao") ? null : reader.GetString("Descricao"),
                                Data = reader.GetDateTime("Data"),
                                CriadoEm = reader.GetDateTime("CriadoEm"),
                                Categoria = new Categoria
                                {
                                    CategoriaID = reader.GetInt32("CategoriaID"),
                                    Nome = reader.GetString("CategoriaNome")
                                }
                            };

                            transacoes.Add(transacao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter transações: " + ex.Message, ex);
            }
            finally
            {
                _connection.Close();
            }

            return transacoes;
        }

        public async Task<Transacao> GetTransacaoById(int id)
        {
            Transacao transacao = null;

            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT t.TransacaoID, t.UsuarioID, t.CategoriaID, t.Tipo, t.Valor, 
                              t.Descricao, t.Data, t.CriadoEm, c.Nome as CategoriaNome
                              FROM Transacoes t
                              INNER JOIN Categorias c ON t.CategoriaID = c.CategoriaID
                              WHERE t.TransacaoID = @id";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (MySqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            transacao = new Transacao
                            {
                                TransacaoID = reader.GetInt32("TransacaoID"),
                                UsuarioID = reader.GetInt32("UsuarioID"),
                                CategoriaID = reader.GetInt32("CategoriaID"),
                                Tipo = reader.GetString("Tipo"),
                                Valor = reader.GetDecimal("Valor"),
                                Descricao = reader.IsDBNull("Descricao") ? null : reader.GetString("Descricao"),
                                Data = reader.GetDateTime("Data"),
                                CriadoEm = reader.GetDateTime("CriadoEm"),
                                Categoria = new Categoria
                                {
                                    CategoriaID = reader.GetInt32("CategoriaID"),
                                    Nome = reader.GetString("CategoriaNome")
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter transação por ID: " + ex.Message, ex);
            }
            finally
            {
                _connection.Close();
            }

            return transacao;
        }

        public async Task AddTransacao(TransacaoDto transacaoDto)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"INSERT INTO Transacoes (UsuarioID, CategoriaID, Tipo, Valor, Descricao, Data, CriadoEm) 
                              VALUES (@UsuarioID, @CategoriaID, @Tipo, @Valor, @Descricao, @Data, @CriadoEm)";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", transacaoDto.UsuarioID);
                    command.Parameters.AddWithValue("@CategoriaID", transacaoDto.CategoriaID);
                    command.Parameters.AddWithValue("@Tipo", transacaoDto.Tipo);
                    command.Parameters.AddWithValue("@Valor", transacaoDto.Valor);
                    command.Parameters.AddWithValue("@Descricao", (object)transacaoDto.Descricao ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Data", transacaoDto.Data);
                    command.Parameters.AddWithValue("@CriadoEm", transacaoDto.CriadoEm);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar transação: " + ex.Message, ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task UpdateTransacao(Transacao transacao)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"UPDATE Transacoes 
                      SET UsuarioID = @UsuarioID, 
                          CategoriaID = @CategoriaID, 
                          Tipo = @Tipo, 
                          Valor = @Valor, 
                          Descricao = @Descricao, 
                          Data = @Data 
                      WHERE TransacaoID = @TransacaoID";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@TransacaoID", transacao.TransacaoID);
                    command.Parameters.AddWithValue("@UsuarioID", transacao.UsuarioID);
                    command.Parameters.AddWithValue("@CategoriaID", transacao.CategoriaID);
                    command.Parameters.AddWithValue("@Tipo", transacao.Tipo);
                    command.Parameters.AddWithValue("@Valor", transacao.Valor);
                    command.Parameters.AddWithValue("@Descricao", (object)transacao.Descricao ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Data", transacao.Data);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar transação: " + ex.Message, ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task DeleteTransacao(int id)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"DELETE FROM Transacoes WHERE TransacaoID = @id";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar transação: " + ex.Message, ex);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}