using ApiLucas.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ApiWeb.Repositorys
{
    public class LembreteRepository : ILembreteRepository, IDisposable
    {
        private readonly MySqlConnection _connection;

        public LembreteRepository(MySqlConnectionDB mySqlConnectionDB)
        {
            _connection = mySqlConnectionDB.CreateConnection();
        }

        // Método para obter lembretes por UsuarioID
        public List<Lembrete> GetLembretesByUsuarioId(int usuarioId)
        {
            List<Lembrete> lembretes = new List<Lembrete>();

            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"SELECT LembreteID, UsuarioID, Titulo, Descricao, DataLembrete, CriadoEm 
                                     FROM lembretes 
                                     WHERE UsuarioID = @UsuarioID";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioID", usuarioId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Lembrete lembrete = new Lembrete
                                {
                                    LembreteID = reader.GetInt32("LembreteID"),
                                    UsuarioID = reader.GetInt32("UsuarioID"),
                                    Titulo = reader.GetString("Titulo"),
                                    Descricao = reader.GetString("Descricao"),
                                    DataLembrete = reader.GetDateTime("DataLembrete"),
                                    CriadoEm = reader.GetDateTime("CriadoEm")
                                };

                                lembretes.Add(lembrete);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return lembretes;
        }

        // Método para obter todos os lembretes
        public List<Lembrete> GetAllLembretes()
        {
            List<Lembrete> lembretes = new List<Lembrete>();

            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"SELECT LembreteID, UsuarioID, Titulo, Descricao, DataLembrete, CriadoEm 
                                     FROM lembretes";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Lembrete lembrete = new Lembrete
                                {
                                    LembreteID = reader.GetInt32("LembreteID"),
                                    UsuarioID = reader.GetInt32("UsuarioID"),
                                    Titulo = reader.GetString("Titulo"),
                                    Descricao = reader.GetString("Descricao"),
                                    DataLembrete = reader.GetDateTime("DataLembrete"),
                                    CriadoEm = reader.GetDateTime("CriadoEm")
                                };

                                lembretes.Add(lembrete);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return lembretes;
        }

        // Dispose para fechar a conexão
        public void Dispose()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
