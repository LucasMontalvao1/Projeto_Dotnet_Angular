﻿using ApiWeb.Infra.Data;
using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using MySqlConnector;
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

                    string query = @"SELECT LembreteID, UsuarioID, Titulo, Descricao, DataLembrete, IntervaloEmDias, CriadoEm 
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
                                    IntervaloEmDias = reader.GetInt32("IntervaloEmDias"),
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

                    string query = @"SELECT LembreteID, UsuarioID, Titulo, Descricao, DataLembrete, IntervaloEmDias, CriadoEm 
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
                                    IntervaloEmDias = reader.GetInt32("IntervaloEmDias"),
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

        public Lembrete GetLembreteById(int lembreteId)
        {
            Lembrete lembrete = null;
            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"SELECT * FROM lembretes WHERE LembreteID = @LembreteID";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@LembreteID", lembreteId);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lembrete = new Lembrete
                                {
                                    LembreteID = reader.GetInt32("LembreteID"),
                                    UsuarioID = reader.GetInt32("UsuarioID"),
                                    Titulo = reader.GetString("Titulo"),
                                    Descricao = reader.GetString("Descricao"),
                                    DataLembrete = reader.GetDateTime("DataLembrete"),
                                    IntervaloEmDias = reader.GetInt32("IntervaloEmDias"),
                                    CriadoEm = reader.GetDateTime("CriadoEm")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter lembrete com ID {lembreteId}: {ex.Message}");
            }
            return lembrete;
        }

        public Lembrete AddLembrete(Lembrete lembrete)
        {
            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"INSERT INTO lembretes (UsuarioID, Titulo, Descricao, DataLembrete, IntervaloEmDias, CriadoEm) 
                                     VALUES (@UsuarioID, @Titulo, @Descricao, @DataLembrete, @IntervaloEmDias, @CriadoEm)";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@UsuarioID", lembrete.UsuarioID);
                        command.Parameters.AddWithValue("@Titulo", lembrete.Titulo);
                        command.Parameters.AddWithValue("@Descricao", lembrete.Descricao);
                        command.Parameters.AddWithValue("@DataLembrete", lembrete.DataLembrete);
                        command.Parameters.AddWithValue("IntervaloEmDias", lembrete.IntervaloEmDias);
                        command.Parameters.AddWithValue("@CriadoEm", lembrete.CriadoEm);

                        command.ExecuteNonQuery();
                        lembrete.LembreteID = (int)command.LastInsertedId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return lembrete;
        }

        public Lembrete UpdateLembrete(Lembrete lembrete)
        {
            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"UPDATE lembretes 
                             SET Titulo = @Titulo, Descricao = @Descricao, DataLembrete = @DataLembrete, IntervaloEmDias = @IntervaloEmDias 
                             WHERE LembreteID = @LembreteID";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@Titulo", lembrete.Titulo);
                        command.Parameters.AddWithValue("@Descricao", lembrete.Descricao);
                        command.Parameters.AddWithValue("@DataLembrete", lembrete.DataLembrete);
                        command.Parameters.AddWithValue("@IntervaloEmDias", lembrete.IntervaloEmDias); 
                        command.Parameters.AddWithValue("@LembreteID", lembrete.LembreteID);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return lembrete;
        }


        public bool DeleteLembrete(int lembreteId)
        {
            try
            {
                using (_connection)
                {
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }

                    string query = @"DELETE FROM lembretes WHERE LembreteID = @LembreteID";

                    using (MySqlCommand command = new MySqlCommand(query, _connection))
                    {
                        command.Parameters.AddWithValue("@LembreteID", lembreteId);

                        int affectedRows = command.ExecuteNonQuery();
                        return affectedRows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
