using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinanceReminderHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _service;

        public TransacaoController(ITransacaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransacoes()
        {
            var transacoes = await _service.GetTransacoes();
            return Ok(transacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransacaoById(int id)
        {
            var transacao = await _service.GetTransacaoById(id);
            if (transacao == null) return NotFound();
            return Ok(transacao);
        }

        [HttpPost]
        public async Task<IActionResult> AddTransacao([FromBody] TransacaoRequest request)
        {
            await _service.AddTransacao(request);
            return CreatedAtAction(nameof(GetTransacoes),  null); // TODO: return CreatedAtRoute
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransacao(int id, [FromBody] TransacaoRequest request)
        {
            await _service.UpdateTransacao(id, request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransacao(int id)
        {
            await _service.DeleteTransacao(id);
            return NoContent();
        }
    }
}
