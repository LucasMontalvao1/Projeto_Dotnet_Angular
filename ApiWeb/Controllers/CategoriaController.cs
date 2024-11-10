using ApiWeb.Models;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Controllers
{
    [Route("api/v1/categoria")]
    [ApiController]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ICategoriaService categoriaService, ILogger<CategoriaController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        // GET: api/categoria
        [HttpGet]
        public async Task<IActionResult> GetAllCategorias()
        {
            try
            {
                IEnumerable<Categoria> categorias = await _categoriaService.GetCategorias();
                return categorias == null ? NotFound("Nenhuma categoria encontrada.") : Ok(categorias);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // GET: api/categoria/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetCategoriaById(id);
                return categoria == null ? NotFound("Categoria não encontrada.") : Ok(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar a categoria com ID {CategoriaID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // POST: api/categoria
        [HttpPost]
        public async Task<IActionResult> AddCategoria([FromBody] Categoria categoria)
        {
            try
            {
                if (categoria == null || string.IsNullOrEmpty(categoria.Nome))
                {
                    return BadRequest("Dados inválidos.");
                }

                int idGerado = await _categoriaService.AddCategoria(categoria);
                return CreatedAtAction(nameof(GetCategoriaById), new { id = idGerado }, categoria); // Use o ID gerado na resposta
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar nova categoria.");
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // PUT: api/categoria/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoria(int id, [FromBody] Categoria categoria)
        {
            try
            {
                if (id != categoria.CategoriaID || categoria == null)
                {
                    return BadRequest("Dados inválidos.");
                }

                await _categoriaService.UpdateCategoria(categoria);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a categoria com ID {CategoriaID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // DELETE: api/categoria/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            try
            {
                await _categoriaService.DeleteCategoria(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar a categoria com ID {CategoriaID}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
