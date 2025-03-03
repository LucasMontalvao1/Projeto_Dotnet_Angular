using ApiWeb.Models.DTOs;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ApiWeb.Tests.Models
{
    public class LoginRequestTests
    {
        [Fact]
        [DisplayName("LoginRequest válido deve passar na validação")]
        public void LoginRequest_Valido_DevePassarNaValidacao()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "usuario.teste",
                Password = "senha123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        [DisplayName("LoginRequest com Username nulo deve falhar na validação")]
        public void LoginRequest_UsernameNulo_DeveFalharNaValidacao()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = null,
                Password = "senha123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Username"));
        }

        [Fact]
        [DisplayName("LoginRequest com Password nulo deve falhar na validação")]
        public void LoginRequest_PasswordNulo_DeveFalharNaValidacao()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "usuario.teste",
                Password = null
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Password"));
        }

        [Fact]
        [DisplayName("LoginRequest com Username muito longo deve falhar na validação")]
        public void LoginRequest_UsernameMuitoLongo_DeveFalharNaValidacao()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = new string('a', 51), // 51 caracteres, excede o limite de 50
                Password = "senha123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Username"));
        }

        [Fact]
        [DisplayName("LoginRequest com Password muito longo deve falhar na validação")]
        public void LoginRequest_PasswordMuitoLongo_DeveFalharNaValidacao()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "usuario.teste",
                Password = new string('a', 101) // 101 caracteres, excede o limite de 100
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.MemberNames.Contains("Password"));
        }

        [Theory]
        [DisplayName("LoginRequest com Username de comprimento variado deve ser validado corretamente")]
        [InlineData("", false)]             // String vazia
        [InlineData("a", true)]             // Valor mínimo (1 caractere)
        [InlineData("usuario", true)]       // Valor comum
        [InlineData("usuario.teste", true)] // Valor com pontuação
        [InlineData("usuário@123", true)]   // Valor com caracteres especiais
        public void LoginRequest_UsernameComprimentoVariado_DeveValidarCorretamente(string username, bool expectedIsValid)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = "senha123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
            if (!expectedIsValid)
            {
                Assert.Contains(validationResults, v => v.MemberNames.Contains("Username"));
            }
        }

        [Theory]
        [DisplayName("LoginRequest com Password de comprimento variado deve ser validado corretamente")]
        [InlineData("", false)]             // String vazia
        [InlineData("a", true)]             // Valor mínimo (1 caractere)
        [InlineData("senha123", true)]      // Valor comum
        [InlineData("senha.com@caracteres!especiais#123", true)]  // Valor com caracteres especiais
        public void LoginRequest_PasswordComprimentoVariado_DeveValidarCorretamente(string password, bool expectedIsValid)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "usuario.teste",
                Password = password
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.Equal(expectedIsValid, isValid);
            if (!expectedIsValid)
            {
                Assert.Contains(validationResults, v => v.MemberNames.Contains("Password"));
            }
        }

        [Fact]
        [DisplayName("LoginRequest com Username no limite máximo deve passar na validação")]
        public void LoginRequest_UsernameNoLimiteMaximo_DevePassarNaValidacao()
        {
            // Arrange
            string usernameMaximo = new string('a', 50); // 50 caracteres (máximo permitido)
            var loginRequest = new LoginRequest
            {
                Username = usernameMaximo,
                Password = "senha123"
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        [DisplayName("LoginRequest com Password no limite máximo deve passar na validação")]
        public void LoginRequest_PasswordNoLimiteMaximo_DevePassarNaValidacao()
        {
            // Arrange
            string passwordMaximo = new string('a', 100); // 100 caracteres (máximo permitido)
            var loginRequest = new LoginRequest
            {
                Username = "usuario.teste",
                Password = passwordMaximo
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(loginRequest,
                new ValidationContext(loginRequest),
                validationResults,
                true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}