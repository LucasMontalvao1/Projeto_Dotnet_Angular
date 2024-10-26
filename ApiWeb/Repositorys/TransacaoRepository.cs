using ApiLucas.Infra.Data;
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

        // Método para obter todas as transações
        public async Task<IEnumerable<Transacao>> GetTransacoes()
        {
            List<Transacao> transacoes = new List<Transacao>();

            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT TransacaoID, UsuarioID, CategoriaID, Tipo, Valor, Descricao, Data, CriadoEm FROM Transacoes";

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
                                CriadoEm = reader.GetDateTime("CriadoEm")
                            };

                            transacoes.Add(transacao);
                        }
                    }
                }
            }
            catch
            {
                throw; 
            }
            finally
            {
                _connection.Close();
            }

            return transacoes;
        }

        // Método para obter uma transação pelo ID
        public async Task<Transacao> GetTransacaoById(int id)
        {
            Transacao transacao = null;

            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"SELECT TransacaoID, UsuarioID, CategoriaID, Tipo, Valor, Descricao, Data, CriadoEm FROM Transacoes WHERE TransacaoID = @id";

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
                                CriadoEm = reader.GetDateTime("CriadoEm")
                            };
                        }
                    }
                }
            }
            catch
            {
                throw; 
            }
            finally
            {
                _connection.Close();
            }

            return transacao;
        }

        // Método para adicionar uma nova transação
        public async Task AddTransacao(TransacaoDto transacaoDto)
        {
            if (transacaoDto.Tipo != "Entrada" && transacaoDto.Tipo != "Saída")
            {
                throw new ArgumentException("Tipo de transação deve ser 'Entrada' ou 'Saída'.");
            }

            bool usuarioExiste = await _authRepository.UsuarioExiste(transacaoDto.UsuarioID);
            bool categoriaExiste = await _categoriaRepository.CategoriaExiste(transacaoDto.CategoriaID);

            if (!usuarioExiste || !categoriaExiste)
            {
                throw new ArgumentException("Usuário ou Categoria não existe.");
            }

            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                var transacao = new Transacao
                {
                    UsuarioID = transacaoDto.UsuarioID,
                    CategoriaID = transacaoDto.CategoriaID,
                    Tipo = transacaoDto.Tipo,
                    Valor = transacaoDto.Valor,
                    Descricao = transacaoDto.Descricao,
                    Data = transacaoDto.Data,
                    CriadoEm = transacaoDto.CriadoEm
                };

                string query = @"INSERT INTO Transacoes (UsuarioID, CategoriaID, Tipo, Valor, Descricao, Data, CriadoEm) 
                                 VALUES (@UsuarioID, @CategoriaID, @Tipo, @Valor, @Descricao, @Data, @CriadoEm)";

                using (MySqlCommand command = new MySqlCommand(query, _connection))
                {
                    command.Parameters.AddWithValue("@UsuarioID", transacao.UsuarioID);
                    command.Parameters.AddWithValue("@CategoriaID", transacao.CategoriaID);
                    command.Parameters.AddWithValue("@Tipo", transacao.Tipo);
                    command.Parameters.AddWithValue("@Valor", transacao.Valor);
                    command.Parameters.AddWithValue("@Descricao", (object)transacao.Descricao ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Data", transacao.Data);
                    command.Parameters.AddWithValue("@CriadoEm", transacao.CriadoEm);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch
            {
                throw; 
            }
            finally
            {
                _connection.Close();
            }
        }

        // Método para atualizar uma transação existente
        public async Task UpdateTransacao(Transacao transacao)
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }

                string query = @"UPDATE Transacoes SET UsuarioID = @UsuarioID, CategoriaID = @CategoriaID, Tipo = @Tipo, Valor = @Valor, Descricao = @Descricao, Data = @Data WHERE TransacaoID = @TransacaoID";

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
            catch
            {
                throw; 
            }
            finally
            {
                _connection.Close();
            }
        }

        // Método para deletar uma transação pelo ID
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
            catch
            {
                throw; 
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
