using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinanceReminderHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LembreteController : ControllerBase
    {
        private readonly ILembreteService _service;

        public LembreteController(ILembreteService service)
        {
            _service = service;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetLembretesByUsuarioId(int usuarioId)
        {
            var lembretes = await _service.GetLembretesByUsuarioId(usuarioId);
            return Ok(lembretes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLembreteById(int id)
        {
            var lembrete = await _service.GetLembreteById(id);
            if (lembrete == null) return NotFound();
            return Ok(lembrete);
        }

        [HttpPost]
        public async Task<IActionResult> AddLembrete([FromBody] LembreteRequest request)
        {
            var id = await _service.AddLembrete(request);
            return CreatedAtAction(nameof(GetLembreteById), new { id = id }, request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLembrete(int id, [FromBody] LembreteRequest request)
        {
            await _service.UpdateLembrete(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLembrete(int id)
        {
            await _service.DeleteLembrete(id);
            return NoContent();
        }
    }
}
