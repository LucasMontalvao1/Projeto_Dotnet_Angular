using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ApiWeb.Services.TransacaoService;

namespace ApiWeb.Controllers
{
    [ApiController]
    [Route("api/v1/transacao")]
    [Authorize]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ILogger<TransacaoController> _logger;

        public TransacaoController(ITransacaoService transacaoService, ILogger<TransacaoController> logger)
        {
            _transacaoService = transacaoService;
            _logger = logger;
        }

        // GET: api/transacao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transacao>>> GetTransacoes()
        {
            try
            {
                _logger.LogInformation("Buscando todas as transações.");
                var transacoes = await _transacaoService.GetTransacoes();
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // GET: api/transacao/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transacao>> GetTransacao(int id)
        {
            try
            {
                _logger.LogInformation("Buscando transação por ID: {TransacaoID}", id);
                var transacao = await _transacaoService.GetTransacaoById(id);
                return transacao is null ? NotFound() : Ok(transacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar a transação com ID {TransacaoID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // POST: api/transacao
        [HttpPost]
        public async Task<ActionResult> PostTransacao([FromBody] TransacaoDto transacaoDTO)
        {
            try
            {
                await _transacaoService.AddTransacao(transacaoDTO);
                _logger.LogInformation("Transação adicionada com sucesso.");
                return CreatedAtAction(nameof(GetTransacao), new { id = transacaoDTO.UsuarioID }, transacaoDTO);
            }
            catch (TipoTransicaoInvalida ex)
            {
                _logger.LogWarning("Erro de validação ao adicionar transação: {ErrorMessage}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (UsuarioCategoriaNaoExiste ex)
            {
                _logger.LogWarning("Erro ao adicionar transação: {ErrorMessage}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar nova transação.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // PUT: api/transacao/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransacao(int id, [FromBody] Transacao transacao)
        {
            try
            {
                if (id != transacao.TransacaoID)
                {
                    _logger.LogWarning("Erro de validação. ID da URL e ID do corpo da requisição não coincidem.");
                    return BadRequest("ID do corpo e da URL precisam coincidir.");
                }

                await _transacaoService.UpdateTransacao(transacao);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a transação com ID {TransacaoID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // DELETE: api/transacao/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            try
            {
                await _transacaoService.DeleteTransacao(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar a transação com ID {TransacaoID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
