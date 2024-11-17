using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
                _logger.LogInformation("Buscando todas as transações com suas categorias.");
                var transacoes = await _transacaoService.GetTransacoes();
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Erro interno do servidor ao buscar transações.");
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

                if (transacao == null)
                {
                    _logger.LogWarning("Transação não encontrada: {TransacaoID}", id);
                    return NotFound($"Transação com ID {id} não encontrada.");
                }

                return Ok(transacao);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Transação não encontrada: {TransacaoID}", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar a transação com ID {TransacaoID}", id);
                return StatusCode(500, "Erro interno do servidor ao buscar transação.");
            }
        }

        // POST: api/transacao
        [HttpPost]
        public async Task<ActionResult> PostTransacao([FromBody] TransacaoDto transacaoDto)
        {
            try
            {
                await _transacaoService.AddTransacao(transacaoDto);
                _logger.LogInformation("Transação adicionada com sucesso para o usuário {UsuarioID}", transacaoDto.UsuarioID);

                return CreatedAtAction(
                    nameof(GetTransacao),
                    new { id = transacaoDto.UsuarioID },
                    transacaoDto
                );
            }
            catch (TipoTransicaoInvalida ex)
            {
                _logger.LogWarning("Erro de validação ao adicionar transação: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (UsuarioCategoriaNaoExiste ex)
            {
                _logger.LogWarning("Erro ao adicionar transação: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não esperado ao adicionar nova transação: {ErrorMessage}", ex.Message);
                return StatusCode(500, "Erro interno do servidor ao adicionar transação.");
            }
        }

        // PUT: api/transacao/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransacao(int id, [FromBody] TransacaoDto transacaoDto)
        {
            try
            {
                if (!transacaoDto.TransacaoID.HasValue || id != transacaoDto.TransacaoID.Value)
                {
                    _logger.LogWarning("ID da URL ({UrlId}) e ID do corpo ({BodyId}) não coincidem.", id, transacaoDto.TransacaoID);
                    return BadRequest(new { error = "ID da URL e ID do corpo da requisição precisam coincidir." });
                }

                await _transacaoService.UpdateTransacao(transacaoDto);
                _logger.LogInformation("Transação {TransacaoID} atualizada com sucesso.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Transação não encontrada para atualização: {TransacaoID}", id);
                return NotFound(new { error = ex.Message });
            }
            catch (UsuarioCategoriaNaoExiste ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao atualizar transação: {ErrorMessage}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a transação {TransacaoID}: {ErrorMessage}", id, ex.Message);
                return StatusCode(500, "Erro interno do servidor ao atualizar transação.");
            }
        }

        // DELETE: api/transacao/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            try
            {
                await _transacaoService.DeleteTransacao(id);
                _logger.LogInformation("Transação {TransacaoID} deletada com sucesso.", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Transação não encontrada para exclusão: {TransacaoID}", id);
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar a transação {TransacaoID}: {ErrorMessage}", id, ex.Message);
                return StatusCode(500, "Erro interno do servidor ao deletar transação.");
            }
        }
    }
}