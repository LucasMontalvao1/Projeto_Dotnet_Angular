using Microsoft.AspNetCore.Mvc;
using ApiWeb.Services.Interfaces;

namespace ApiWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HangfireController : ControllerBase
    {
        private readonly IHangfireService _hangfireService;
        private readonly ILogger<HangfireController> _logger; 

        public HangfireController(IHangfireService hangfireService, ILogger<HangfireController> logger)
        {
            _hangfireService = hangfireService;
            _logger = logger; 
        }

        [HttpPost("disparar-job")]
        public IActionResult DispararJob()
        {
            try
            {
                _hangfireService.EnqueueJob();
                _logger.LogInformation("Job 'VerificarLembretesRepetidos' disparada com sucesso.");
                return Ok(new { Message = "Job 'VerificarLembretesRepetidos' disparada com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao disparar o job 'VerificarLembretesRepetidos'.");
                return StatusCode(500, new { Message = "Ocorreu um erro ao disparar o job." });
            }
        }
    }
}
