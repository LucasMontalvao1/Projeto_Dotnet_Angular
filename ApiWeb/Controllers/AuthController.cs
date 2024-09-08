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

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var usuario = _authService.ValidarUsuario(user);
            if (usuario != null)
            {
                var token = _authService.GenerateToken(usuario);
                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        user.UsuarioID,
                        user.Username,
                        user.Name,
                        user.Foto,
                        user.Email
                    }
                });
            }
            return Unauthorized("Login ou senha incorretos");
        }
    }
}

