using ApiWeb.Models;
using ApiWeb.Services.Interfaces;
using Moq;

namespace ApiWeb.Tests.Mocks
{
    public class MockLembreteService
    {
        public static Mock<ILembreteService> GetMock()
        {
            var mockService = new Mock<ILembreteService>();

            // Configuração para GetAllLembretes
            mockService.Setup(service => service.GetAllLembretes())
                .Returns(new List<Lembrete>
                {
                    new Lembrete { LembreteID = 1, Titulo = "Lembrete Teste 1" },
                    new Lembrete { LembreteID = 2, Titulo = "Lembrete Teste 2" }
                });

            // Configuração para GetLembretesByUsuarioId
            mockService.Setup(service => service.GetLembretesByUsuarioId(It.IsAny<int>()))
                .Returns((int usuarioId) => new List<Lembrete>
                {
                    new Lembrete { LembreteID = usuarioId, Titulo = $"Lembrete Usuario {usuarioId}" }
                });

            // Configuração para GetLembreteById
            mockService.Setup(service => service.GetLembreteById(It.IsAny<int>()))
                .Returns((int id) =>
                {
                    if (id == 1)
                        return new Lembrete { LembreteID = 1, Titulo = "Lembrete Teste" };
                    return null; // Simula lembrete inexistente
                });

            // Configuração para AddLembrete
            mockService.Setup(service => service.AddLembrete(It.IsAny<Lembrete>(), It.IsAny<string>()))
                .Returns((Lembrete lembrete, string userId) =>
                {
                    if (lembrete.Titulo == "Lembrete Teste 1") // Simula que já existe
                        throw new Exception("Lembrete já existe");
                    lembrete.LembreteID = 3; // Simula ID gerado
                    return lembrete;
                });

            // Configuração para DeleteLembrete
            mockService.Setup(service => service.DeleteLembrete(It.IsAny<int>()))
                .Returns((int id) => id == 1); // Suponha que apenas o ID 1 pode ser excluído

            return mockService;
        }
    }
}
