using ApiWeb.Controllers;
using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Services.Interfaces;
using ApiWeb.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

namespace ApiWeb.Tests.Controllers
{
    /// <summary>
    /// Testes unitários para o AuthController
    /// </summary>
    public class AuthControllerTests
    {
        // Campos privados para mocks e controller
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        // Dados de teste comuns
        private readonly UserDto _validUserDto;
        private readonly User _validUser;

        public AuthControllerTests()
        {
            // Configuração inicial dos mocks
            _authServiceMock = MockAuthService.GetMock();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);

            // Inicialização dos dados de teste comuns
            _validUserDto = new UserDto { Username = "usuario", Password = "senha" };
            _validUser = new User
            {
                UsuarioID = 1,
                Username = "usuario",
                Name = "Nome",
                Foto = "foto.jpg",
                Email = "email@dominio.com"
            };
        }

        #region Testes de Login Bem-Sucedido

        [Fact(DisplayName = "Testa o usuario valido se retorna ok")]
        public void Login_UsuarioValido_DeveRetornarOk()
        {
            // Arrange
            ConfigurarLoginBemSucedido();

            // Act
            var result = _controller.Login(_validUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<LoginResponse>(okResult.Value);

            Assert.NotNull(response);
            Assert.NotNull(response.Token);
            Assert.NotNull(response.User);
            Assert.Equal(_validUser.Username, response.User.Username);
        }

        [Fact(DisplayName = "Testa login sucesso e se retorna token valido")]
        public void Login_UsuarioValido_DeveRetornarTokenValido()
        {
            // Arrange
            var token = "token-gerado";
            ConfigurarLoginBemSucedido(token);

            // Act
            var result = _controller.Login(_validUserDto) as OkObjectResult;

            // Assert
            var response = Assert.IsAssignableFrom<LoginResponse>(result.Value);
            Assert.Equal(token, response.Token);
            Assert.False(string.IsNullOrEmpty(response.Token), "Token não deve ser vazio");
        }

        [Fact (DisplayName = "Testa usaurio valida e valida o usuario")]
        public void Login_UsuarioValido_DeveChamarValidarUsuario()
        {
            // Arrange
            ConfigurarLoginBemSucedido();

            // Act
            _controller.Login(_validUserDto);

            // Assert
            _authServiceMock.Verify(a => a.ValidarUsuario(_validUserDto.Username, _validUserDto.Password), Times.Once);
        }

        #endregion

        #region Testes de Falha de Login

        [Fact]
        public void Login_UsuarioInvalido_DeveRetornarUnauthorized()
        {
            // Arrange
            _authServiceMock.Setup(a => a.ValidarUsuario(_validUserDto.Username, _validUserDto.Password))
                .Returns((User)null);

            // Act
            var result = _controller.Login(_validUserDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal("Login ou senha incorretos", unauthorizedResult.Value);
        }

        [Theory]
        [InlineData(null, "senha")]
        [InlineData("usuario", null)]
        [InlineData("", "senha")]
        [InlineData("usuario", "")]
        public void Login_DadosInvalidos_DeveRetornarBadRequest(string username, string password)
        {
            // Arrange
            var userDto = new UserDto { Username = username, Password = password };

            // Act
            var result = _controller.Login(userDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nome de usuário e senha não podem estar vazios", badRequestResult.Value);
        }

        [Fact]
        public void Login_UserDtoNulo_DeveRetornarBadRequest()
        {
            // Act
            var result = _controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Usuário não pode ser nulo", badRequestResult.Value);
        }

        #endregion

        #region Testes de Erro e Logging

        [Fact]
        public void Login_ErroAoValidarUsuario_DeveRetornar500()
        {
            // Arrange
            _authServiceMock.Setup(a => a.ValidarUsuario(_validUserDto.Username, _validUserDto.Password))
                .Throws(new Exception("Erro interno"));

            // Act
            var result = _controller.Login(_validUserDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public void Login_ErroAoValidarUsuario_DeveLogarErro()
        {
            // Arrange
            _authServiceMock.Setup(a => a.ValidarUsuario(_validUserDto.Username, _validUserDto.Password))
                .Throws(new Exception("Erro interno"));

            // Act
            _controller.Login(_validUserDto);

            // Assert
            VerificarLogErro();
        }

        [Fact]
        public void Login_LoginBemSucedido_DeveLogarSucesso()
        {
            // Arrange
            ConfigurarLoginBemSucedido();

            // Act
            _controller.Login(_validUserDto);

            // Assert
            VerificarLogSucesso();
        }

        #endregion

        #region Métodos Auxiliares

        private void ConfigurarLoginBemSucedido(string token = "token-valido")
        {
            _authServiceMock.Setup(a => a.ValidarUsuario(_validUserDto.Username, _validUserDto.Password))
                .Returns(_validUser);
            _authServiceMock.Setup(a => a.GenerateToken(_validUser))
                .Returns(token);
        }

        private void VerificarLogErro()
        {
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        private void VerificarLogSucesso()
        {
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Login bem-sucedido para o usuário")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion
    }
}