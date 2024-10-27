using ApiWeb.Models;
using ApiWeb.Services.Interfaces;
using Moq;

namespace ApiWeb.Tests.Mocks
{
    public class MockAuthService
    {
        public static Mock<IAuthService> GetMock()
        {
            var mockService = new Mock<IAuthService>();

            // Configuração para um usuário válido
            mockService.Setup(service => service.ValidarUsuario(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string username, string password) =>
                {
                    if (username == "usuario" && password == "senha")
                    {
                        return new User
                        {
                            UsuarioID = 1,
                            Username = "usuario",
                            Name = "Nome",
                            Foto = "foto.jpg",
                            Email = "email@dominio.com"
                        };
                    }
                    return null; // Usuário inválido
                });

            // Configuração para geração de token
            mockService.Setup(service => service.GenerateToken(It.IsAny<User>()))
                .Returns("token");

            return mockService;
        }
    }
}
