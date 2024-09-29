using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Security.Claims;

namespace ApiWeb.Controllers
{
    [ApiController]
    [Route("api/v1/lembretes")]
    [Authorize] // Exige autenticação
    public class LembreteController : ControllerBase
    {
        private readonly ILembreteService _lembreteService;
        private readonly ILogger<LembreteController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LembreteController(ILembreteService lembreteService, ILogger<LembreteController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _lembreteService = lembreteService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        // Método para obter todos os lembretes
        [HttpGet("todos")]
        public IActionResult GetAllLembretes()
        {
            try
            {
                var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                Log.Information("Usuário com ID {UsuarioId} fez uma requisição para obter todos os lembretes.", usuarioId);
                Log.Information("Usuário com nome {Name} fez uma requisição para obter todos os lembretes.", name);

                var lembretes = _lembreteService.GetAllLembretes(); // Método que deve ser implementado no serviço

                if (lembretes == null || lembretes.Count == 0)
                {
                    _logger.LogInformation("Nenhum lembrete encontrado.");
                    return NotFound("Nenhum lembrete encontrado.");
                }

                return Ok(lembretes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter todos os lembretes.");
                return StatusCode(500, $"Ocorreu um erro ao processar sua requisição: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetLembretes()
        {
            try
            {
                // Verifica se o usuário está autenticado
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    _logger.LogWarning("Token inválido ou não encontrado.");
                    return Unauthorized("Token inválido ou não encontrado. Por favor, inclua um token válido.");
                }

                // Converte o ID do usuário autenticado para int
                if (!int.TryParse(userIdClaim.Value, out int usuarioId))
                {
                    _logger.LogWarning("ID do usuário inválido no token.");
                    return Unauthorized("ID do usuário inválido no token.");
                }

                // Chama o serviço para obter os lembretes do usuário
                var lembretes = _lembreteService.GetLembretesByUsuarioId(usuarioId);

                if (lembretes == null || lembretes.Count == 0)
                {
                    _logger.LogInformation($"Nenhum lembrete encontrado para o usuário com ID {usuarioId}.");
                    return NotFound("Nenhum lembrete encontrado para o usuário.");
                }

                return Ok(lembretes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Erro ao processar o ID do usuário.");
                return BadRequest($"Erro ao processar o ID do usuário: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Acesso negado.");
                return Unauthorized($"Acesso negado: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao processar sua requisição.");
                return StatusCode(500, $"Ocorreu um erro ao processar sua requisição: {ex.Message}");
            }
        }
    }
}
