using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services;
using Moq;
using Xunit;

namespace ApiWeb.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<ITokenRepository> _tokenRepositoryMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _authService = new AuthService(_authRepositoryMock.Object, _tokenRepositoryMock.Object);
        }

        [Fact(DisplayName = "ValidarUsuario deve retornar um usuário com credenciais válidas")]
        public void ValidarUsuario_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var password = "password";
            var expectedUser = new User { Username = username, Password = password };

            _authRepositoryMock.Setup(repo => repo.ValidarUsuario(username, password)).Returns(expectedUser);

            // Act
            var result = _authService.ValidarUsuario(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact(DisplayName = "ValidarUsuario deve lançar exceção com credenciais inválidas")]
        public void ValidarUsuario_InvalidCredentials_ThrowsException()
        {
            // Arrange
            var username = "invaliduser";
            var password = "wrongpassword";

            _authRepositoryMock.Setup(repo => repo.ValidarUsuario(username, password)).Throws(new Exception("Usuário não encontrado"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _authService.ValidarUsuario(username, password));
            Assert.Contains("Erro ao validar usuário", ex.Message);
        }

        [Fact(DisplayName = "GenerateToken deve retornar um token para usuário válido")]
        public void GenerateToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var user = new User { Username = "testuser" };
            var expectedToken = "valid_token";

            _tokenRepositoryMock.Setup(repo => repo.CreateToken(user)).Returns(expectedToken);

            // Act
            var result = _authService.GenerateToken(user);

            // Assert
            Assert.Equal(expectedToken, result);
        }

        [Fact(DisplayName = "GenerateToken deve lançar exceção ao falhar na criação do token")]
        public void GenerateToken_TokenCreationFails_ThrowsException()
        {
            // Arrange
            var user = new User { Username = "testuser" };

            _tokenRepositoryMock.Setup(repo => repo.CreateToken(user)).Throws(new Exception("Erro interno"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _authService.GenerateToken(user));
            Assert.Contains("Erro ao gerar token", ex.Message);
        }

        [Fact(DisplayName = "ValidateToken deve retornar verdadeiro para um token válido")]
        public void ValidateToken_ValidToken_ReturnsTrue()
        {
            // Arrange
            var token = "valid_token";

            _tokenRepositoryMock.Setup(repo => repo.IsValidToken(token)).Returns(true);

            // Act
            var result = _authService.ValidateToken(token);

            // Assert
            Assert.True(result);
        }

        [Fact(DisplayName = "ValidateToken deve retornar falso para um token inválido")]
        public void ValidateToken_InvalidToken_ReturnsFalse()
        {
            // Arrange
            var token = "invalid_token";

            _tokenRepositoryMock.Setup(repo => repo.IsValidToken(token)).Returns(false);

            // Act
            var result = _authService.ValidateToken(token);

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "ValidateToken deve lançar exceção ao falhar na validação do token")]
        public void ValidateToken_TokenValidationFails_ThrowsException()
        {
            // Arrange
            var token = "error_token";

            _tokenRepositoryMock.Setup(repo => repo.IsValidToken(token)).Throws(new Exception("Token inválido"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _authService.ValidateToken(token));
            Assert.Contains("Erro ao validar token", ex.Message);
        }
    }
}
