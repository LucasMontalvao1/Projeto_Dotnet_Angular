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
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;
        private readonly UserDto _validUserDto;
        private readonly User _validUser;

        public AuthControllerTests()
        {
            _authServiceMock = MockAuthService.GetMock();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);

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

        [Fact(DisplayName = "Login - Deve retornar OK e dados do usuário quando credenciais são válidas")]
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

        [Fact(DisplayName = "Login - Deve retornar token válido quando autenticação é bem-sucedida")]
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

        [Fact(DisplayName = "Login - Deve chamar ValidarUsuario uma vez com credenciais corretas")]
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

        [Fact(DisplayName = "Login - Deve retornar Unauthorized quando credenciais são inválidas")]
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

        [Theory(DisplayName = "Login - Deve retornar BadRequest para dados de login inválidos")]
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

        [Fact(DisplayName = "Login - Deve retornar BadRequest quando UserDto é nulo")]
        public void Login_UserDtoNulo_DeveRetornarBadRequest()
        {
            // Act
            var result = _controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Usuário não pode ser nulo", badRequestResult.Value);
        }

        [Fact(DisplayName = "Login - Deve retornar Unauthorized quando senha está incorreta")]
        public void Login_SenhaIncorreta_DeveRetornarUnauthorized()
        {
            // Arrange
            _authServiceMock.Setup(a => a.ValidarUsuario(_validUserDto.Username, "senhaErrada"))
                .Returns((User)null);

            var invalidUserDto = new UserDto { Username = _validUserDto.Username, Password = "senhaErrada" };

            // Act
            var result = _controller.Login(invalidUserDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            Assert.Equal("Login ou senha incorretos", unauthorizedResult.Value);
        }

        [Fact(DisplayName = "Login - Deve suportar múltiplos logins simultâneos")]
        public void Login_MultiplosLogins_DeveSerProcessadoCorretamente()
        {
            // Arrange
            ConfigurarLoginBemSucedido();

            Parallel.For(0, 100, i =>
            {
                // Act
                var result = _controller.Login(_validUserDto);

                // Assert
                Assert.IsType<OkObjectResult>(result);
            });
        }


        #endregion

        #region Testes de Erro e Logging

        [Fact(DisplayName = "Login - Deve retornar StatusCode 500 quando ocorre erro interno")]
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

        [Fact(DisplayName = "Login - Deve registrar log de erro quando ocorre exceção")]
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

        [Fact(DisplayName = "Login - Deve registrar log de sucesso após login bem-sucedido")]
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