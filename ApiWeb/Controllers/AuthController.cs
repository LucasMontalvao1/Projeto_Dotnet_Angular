﻿using ApiWeb.Models;
using ApiWeb.Models.DTOs;
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
            // Verifica se userDto é nulo
            if (userDto == null)
            {
                return BadRequest("Usuário não pode ser nulo");
            }

            // Verifica se Username ou Password estão vazios
            if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password))
            {
                return BadRequest("Nome de usuário e senha não podem estar vazios");
            }

            _logger.LogInformation("Tentativa de login para o usuário: {Username}", userDto.Username);

            try
            {
                var usuario = _authService.ValidarUsuario(userDto.Username, userDto.Password);
                if (usuario != null)
                {
                    var token = _authService.GenerateToken(usuario);

                    _logger.LogInformation("Login bem-sucedido para o usuário: {Username}", usuario.Username);

                    return Ok(new LoginResponse
                    {
                        Token = token,
                        User = new UserResponseDto
                        {
                            UsuarioID = usuario.UsuarioID,
                            Username = usuario.Username,
                            Name = usuario.Name,
                            Foto = usuario.Foto,
                            Email = usuario.Email
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
