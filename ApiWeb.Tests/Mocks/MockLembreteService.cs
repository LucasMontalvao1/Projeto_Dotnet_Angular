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

            // Adicione mais setups conforme necessário
            return mockService;
        }
    }
}
