using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ApiWeb.Controllers;
using ApiWeb.Models;
using ApiWeb.Services.Interfaces;
using ApiWeb.Tests.Mocks;
using ApiWeb.Tests.TestUtils;
using ApiWeb.Tests.Fixtures;
using System.Collections.Generic;
using ApiWeb.Helpers;
using System.Security.Claims;

namespace ApiWeb.Tests.Controllers
{
    public class LembreteControllerTests : IClassFixture<DatabaseFixture>
    {
        private readonly Mock<ILembreteService> _lembreteServiceMock;
        private readonly Mock<ILogger<LembreteController>> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly LembreteController _controller;
        private readonly DatabaseFixture _fixture;

        public LembreteControllerTests(DatabaseFixture fixture)
        {
            _fixture = fixture; // Usando o DatabaseFixture
            _lembreteServiceMock = MockLembreteService.GetMock(); // Usando MockLembreteService
            _loggerMock = new Mock<ILogger<LembreteController>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(FakeHttpContext.CreateFakeContext("1")); // Usando FakeHttpContext

            _controller = new LembreteController(_lembreteServiceMock.Object, _loggerMock.Object, httpContextAccessor.Object);
        }

        [Fact]
        public void GetAllLembretes_ShouldReturnAllLembretes()
        {
            // Act
            var result = _controller.GetAllLembretes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var lembretes = Assert.IsAssignableFrom<List<Lembrete>>(okResult.Value);
            Assert.Equal(2, lembretes.Count); // Correspondendo ao mock setup
        }

        [Fact]
        public void GetLembretes_ShouldReturnUserLembretes()
        {
            // Arrange
            var usuarioId = 1; // ID do usuário para o teste
            var lembretes = new List<Lembrete>
            {
                new Lembrete { LembreteID = 1, UsuarioID = usuarioId, Titulo = "Lembrete Usuario 1", Descricao = "Descrição 1" }
            };

            // Configurando o mock para retornar a lista de lembretes do usuário
            _lembreteServiceMock.Setup(service => service.GetLembretesByUsuarioId(usuarioId)).Returns(lembretes);

            // Configurando o contexto do controlador com o usuário autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = _controller.GetLembretes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLembretes = Assert.IsAssignableFrom<List<Lembrete>>(okResult.Value);
            Assert.Single(returnedLembretes);
            Assert.Equal("Lembrete Usuario 1", returnedLembretes[0].Titulo);
        }

        [Fact]
        public void GetLembreteById_ExistingId_ShouldReturnLembrete()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.GetLembreteById(1))
                .Returns(new Lembrete { LembreteID = 1, Titulo = "Lembrete Teste" });

            // Act
            var result = _controller.GetLembreteById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var lembrete = Assert.IsType<Lembrete>(okResult.Value);
            Assert.Equal("Lembrete Teste", lembrete.Titulo);
        }

        [Fact]
        public void AddLembrete_ValidLembrete_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var newLembrete = new Lembrete { Titulo = "Novo Lembrete" };
            _lembreteServiceMock.Setup(service => service.AddLembrete(newLembrete, It.IsAny<string>()))
                .Returns(new Lembrete { LembreteID = 3, Titulo = "Novo Lembrete" });

            // Act
            var result = _controller.AddLembrete(newLembrete);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var lembrete = Assert.IsType<Lembrete>(createdAtActionResult.Value);
            Assert.Equal("Novo Lembrete", lembrete.Titulo);
        }
            
        
        [Fact]
        public void AddLembrete_ServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var newLembrete = new Lembrete { Titulo = "Novo Lembrete" };
            _lembreteServiceMock.Setup(service => service.AddLembrete(newLembrete, It.IsAny<string>()))
                .Throws(new Exception("Erro ao adicionar lembrete."));

            // Act
            var result = _controller.AddLembrete(newLembrete);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro ao adicionar novo lembrete", statusCodeResult.Value.ToString());
        }


        [Fact]
        public void DeleteLembrete_ExistingId_ShouldReturnNoContent()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.DeleteLembrete(1)).Returns(true);

            // Act
            var result = _controller.DeleteLembrete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CalculateDateDifference_ShouldReturnCorrectDifference()
        {
            // Arrange
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 1, 10);

            // Act
            var difference = DateHelper.CalculateDateDifference(startDate, endDate);

            // Assert
            Assert.Equal(9, difference);
        }

        [Fact]
        public void GetLembretes_NoLembretesForUser_ShouldReturnEmptyList()
        {
            // Arrange
            var usuarioId = 1;
            _lembreteServiceMock.Setup(service => service.GetLembretesByUsuarioId(usuarioId)).Returns(new List<Lembrete>());

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
    };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = _controller.GetLembretes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLembretes = Assert.IsAssignableFrom<List<Lembrete>>(okResult.Value);
            Assert.Empty(returnedLembretes);
        }


        [Fact]
        public void GetLembreteById_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.GetLembreteById(999)).Returns((Lembrete)null);

            // Act
            var result = _controller.GetLembreteById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Lembrete com ID 999 não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public void DeleteLembrete_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.DeleteLembrete(999)).Returns(false);

            // Act
            var result = _controller.DeleteLembrete(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Lembrete com ID 999 não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public void GetLembretes_NoLembretes_ShouldReturnEmptyList()
        {
            // Arrange
            var usuarioId = 1;
            _lembreteServiceMock.Setup(service => service.GetLembretesByUsuarioId(usuarioId)).Returns(new List<Lembrete>());

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = _controller.GetLembretes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedLembretes = Assert.IsAssignableFrom<List<Lembrete>>(okResult.Value);
            Assert.Empty(returnedLembretes);
        }

        

    }
}
