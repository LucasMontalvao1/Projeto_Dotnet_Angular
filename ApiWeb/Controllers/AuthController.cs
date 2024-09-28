using ApiWeb.Models;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiWeb.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto userDto)
        {

            _logger.LogInformation("Tentativa de login para o usuário: {Username}", userDto.Username);

            try
            {
            
                var usuario = _authService.ValidarUsuario(userDto.Username, userDto.Password);
                if (usuario != null)
                {
                    var token = _authService.GenerateToken(usuario);
                    
                    _logger.LogInformation("Login bem-sucedido para o usuário: {Username}", usuario.Username);

                    return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        usuario.UsuarioID,
                        usuario.Username,
                        usuario.Name,
                        usuario.Foto,
                        usuario.Email
                    }
                });
            }

                _logger.LogWarning("Falha no login para o usuário: {Username}", userDto.Username);
                return Unauthorized("Login ou senha incorretos");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar realizar login para o usuário: {Username}", userDto.Username);
                return StatusCode(500, "Ocorreu um erro interno no servidor.");
            }
        }
    }
}
