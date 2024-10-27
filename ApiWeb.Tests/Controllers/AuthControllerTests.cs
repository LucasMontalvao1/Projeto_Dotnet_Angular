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

        [Fact]
        public void Login_UserDtoNulo_DeveRetornarBadRequest()
        {
            // Arrange
            UserDto userDto = null;

            // Act
            var result = _controller.Login(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Usuário não pode ser nulo", badRequestResult.Value);
        }

        [Fact]
        public void Login_DadosDeLoginVazios_DeveRetornarBadRequest()
        {
            // Arrange
            var userDto = new UserDto { Username = "", Password = "" };

            // Act
            var result = _controller.Login(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nome de usuário e senha não podem estar vazios", badRequestResult.Value);
        }

        [Fact]
        public void Login_UsuarioValido_ChamaValidarUsuario()
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

            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Returns(usuario);

            // Act
            _controller.Login(userDto);

            // Assert
            _authServiceMock.Verify(a => a.ValidarUsuario(userDto.Username, userDto.Password), Times.Once);
        }

        [Fact]
        public void Login_UsuarioValido_DeveRetornarTokenValido()
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

            var token = "token-gerado";
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Returns(usuario);
            _authServiceMock.Setup(a => a.GenerateToken(usuario)).Returns(token); // Supondo que você tenha esse método

            // Act
            var result = _controller.Login(userDto) as OkObjectResult;

            // Assert
            var responseValue = Assert.IsAssignableFrom<LoginResponse>(result.Value);
            Assert.Equal(token, responseValue.Token);
        }

        [Fact]
        public void Login_ErroAoValidarUsuario_LogErro()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Throws(new Exception("Erro interno"));

            // Act
            var result = _controller.Login(userDto) as ObjectResult;

            // Assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void Login_UsuarioValido_LogSucesso()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            var usuario = new User { UsuarioID = 1, Username = "usuario", Name = "Nome", Foto = "foto.jpg", Email = "email@dominio.com" };
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Returns(usuario);

            // Act
            _controller.Login(userDto);

            // Assert
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Login bem-sucedido para o usuário")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void Login_UsuarioValido_TokenNaoDeveSerVazio()
        {
            // Arrange
            var userDto = new UserDto { Username = "usuario", Password = "senha" };
            var usuario = new User { UsuarioID = 1, Username = "usuario", Name = "Nome", Foto = "foto.jpg", Email = "email@dominio.com" };
            _authServiceMock.Setup(a => a.ValidarUsuario(userDto.Username, userDto.Password)).Returns(usuario);
            _authServiceMock.Setup(a => a.GenerateToken(usuario)).Returns("token-gerado");

            // Act
            var result = _controller.Login(userDto) as OkObjectResult;

            // Assert
            var responseValue = Assert.IsAssignableFrom<LoginResponse>(result.Value);
            Assert.False(string.IsNullOrEmpty(responseValue.Token), "Token não deve ser vazio");
        }

        [Fact]
        public void Login_UsernameOuPasswordNulo_DeveRetornarBadRequest()
        {
            // Arrange
            var userDto = new UserDto { Username = null, Password = "senha" }; // ou Password nulo

            // Act
            var result = _controller.Login(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nome de usuário e senha não podem estar vazios", badRequestResult.Value);
        }
    }
}
