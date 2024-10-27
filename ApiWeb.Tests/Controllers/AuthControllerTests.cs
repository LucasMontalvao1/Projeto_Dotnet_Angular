using ApiWeb.Controllers;
using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Services.Interfaces;
using ApiWeb.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApiWeb.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = MockAuthService.GetMock();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Login_UsuarioValido_DeveRetornarOk()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            var usuario = new User
            {
                UsuarioID = 1,
                Username = "usuario",
                Name = "Nome",
                Foto = "foto.jpg",
                Email = "email@dominio.com"
            };

            // Act
            var result = _controller.Login(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseValue = Assert.IsAssignableFrom<LoginResponse>(okResult.Value);

            Assert.NotNull(responseValue);
            Assert.NotNull(responseValue.Token);
            Assert.NotNull(responseValue.User);
            Assert.Equal("usuario", responseValue.User.Username);
        }

        [Fact]
        public void Login_UsuarioInvalido_DeveRetornarUnauthorized()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Returns((User)null);

            // Act
            var result = _controller.Login(userDto) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Login ou senha incorretos", result.Value);
        }

        [Fact]
        public void Login_ErroAoValidarUsuario_DeveRetornar500()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Throws(new Exception("Erro interno"));

            // Act
            var result = _controller.Login(userDto) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }
    }
}
