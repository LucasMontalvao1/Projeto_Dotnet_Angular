using ApiWeb.Models;
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
    [Authorize]
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

        // GET: api/v1/lembretes/todos
        [HttpGet("todos")]
        public IActionResult GetAllLembretes()
        {
            try
            {
                var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Usuário com ID {usuarioId} está buscando todos os lembretes.");

                var lembretes = _lembreteService.GetAllLembretes();

                if (lembretes == null || lembretes.Count == 0)
                {
                    _logger.LogInformation("Nenhum lembrete encontrado.");
                    return Ok(new List<Lembrete>());
                }

                _logger.LogInformation($"Encontrados {lembretes.Count} lembretes.");
                return Ok(lembretes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao obter todos os lembretes.");
                return StatusCode(500, $"Ocorreu um erro ao processar sua requisição: {ex.Message}");
            }
        }

        // GET: api/v1/lembretes
        [HttpGet]
        public IActionResult GetLembretes()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var usuarioId = userIdClaim?.Value;

                if (usuarioId == null)
                {
                    _logger.LogWarning("Token inválido ou não encontrado.");
                    return Unauthorized("Token inválido ou não encontrado. Por favor, inclua um token válido.");
                }

                _logger.LogInformation($"Usuário com ID {usuarioId} está buscando seus lembretes.");

                var lembretes = _lembreteService.GetLembretesByUsuarioId(int.Parse(usuarioId));

                
                if (lembretes == null || lembretes.Count == 0)
                {
                    _logger.LogInformation($"Nenhum lembrete encontrado para o usuário com ID {usuarioId}.");
                    return Ok(new List<Lembrete>()); 
                }

                _logger.LogInformation($"Usuário com ID {usuarioId} encontrou {lembretes.Count} lembretes.");
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
                _logger.LogError(ex, "Ocorreu um erro ao processar a requisição.");
                return StatusCode(500, $"Ocorreu um erro ao processar a requisição: {ex.Message}");
            }
        }

        // GET: api/v1/lembretes/{id}
        [HttpGet("{id}")]
        public IActionResult GetLembreteById(int id)
        {
            try
            {
                var lembrete = _lembreteService.GetLembreteById(id);
                if (lembrete == null)
                {
                    return NotFound($"Lembrete com ID {id} não encontrado.");
                }
                return Ok(lembrete);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao obter lembrete: {ex.Message}");
            }
        }


        // POST: api/v1/lembretes
        [HttpPost]
        public IActionResult AddLembrete([FromBody] Lembrete lembrete)
        {
            if (lembrete == null || string.IsNullOrWhiteSpace(lembrete.Titulo))
            {
                return BadRequest("Lembrete inválido");
            }

            try
            {
                var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Usuário com ID {usuarioId} está adicionando um novo lembrete.");

                var novoLembrete = _lembreteService.AddLembrete(lembrete, $"Lembrete criado com intervalo de {lembrete.IntervaloEmDias} dias.");

                _logger.LogInformation($"Novo lembrete criado com ID {novoLembrete.LembreteID} pelo usuário com ID {usuarioId}.");

                return CreatedAtAction(nameof(GetLembretes), new { id = novoLembrete.LembreteID }, novoLembrete);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar novo lembrete.");
                return StatusCode(500, $"Erro ao adicionar novo lembrete: {ex.Message}");
            }
        }


        // PUT: api/v1/lembretes/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateLembrete(int id, [FromBody] Lembrete lembrete)
        {
            try
            {
                var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Usuário com ID {usuarioId} está atualizando o lembrete com ID {id}.");

                if (id != lembrete.LembreteID)
                {
                    _logger.LogWarning($"ID do lembrete ({id}) não corresponde ao ID fornecido no corpo da requisição ({lembrete.LembreteID}).");
                    return BadRequest("ID do lembrete não corresponde.");
                }

                var lembreteAtualizado = _lembreteService.UpdateLembrete(lembrete);
                if (lembreteAtualizado == null)
                {
                    _logger.LogWarning($"Lembrete com ID {id} não encontrado para atualização.");
                    return NotFound($"Lembrete com ID {id} não encontrado.");
                }

                _logger.LogInformation($"Lembrete com ID {id} foi atualizado com sucesso pelo usuário com ID {usuarioId}.");
                return Ok(lembreteAtualizado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar o lembrete com ID {id}.");
                return StatusCode(500, $"Erro ao atualizar lembrete: {ex.Message}");
            }
        }

        // DELETE: api/v1/lembretes/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteLembrete(int id)
        {
            try
            {
                var usuarioId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Usuário com ID {usuarioId} está tentando deletar o lembrete com ID {id}.");

                var sucesso = _lembreteService.DeleteLembrete(id);
                if (!sucesso)
                {
                    _logger.LogWarning($"Lembrete com ID {id} não encontrado para exclusão.");
                    return NotFound($"Lembrete com ID {id} não encontrado.");
                }

                _logger.LogInformation($"Lembrete com ID {id} foi deletado com sucesso pelo usuário com ID {usuarioId}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao deletar o lembrete com ID {id}.");
                return StatusCode(500, $"Erro ao deletar lembrete: {ex.Message}");
            }
        }
    }
}
