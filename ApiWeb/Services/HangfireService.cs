using ApiWeb.Services.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Microsoft.Extensions.Configuration;
using System;

namespace ApiWeb.Services.Hangfire
{
    public class HangfireService : IHangfireService
    {
        private readonly ILogger<HangfireService> _logger;
        private readonly string _connectionString;

        public HangfireService(ILogger<HangfireService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("ConfiguracaoPadrao");
        }

        public void EnqueueJob()
        {
            BackgroundJob.Enqueue(() => ExecuteJob());
        }

        public void ExecuteJob()
        {
            _logger.LogInformation("Iniciando a execução da job 'VerificarLembretesRepetidos'...");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var command = new MySqlCommand("CALL VerificarLembretesRepetidos", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                _logger.LogInformation("Job 'VerificarLembretesRepetidos' executada com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar a job 'VerificarLembretesRepetidos'.");
            }
        }
    }
}
