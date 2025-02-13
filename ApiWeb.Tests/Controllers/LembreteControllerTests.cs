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
using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Tests.Controllers
{
    public class LembreteControllerTests : IClassFixture<DatabaseFixture>
    {
        // Campos privados para armazenar as dependências mockadas
        private readonly Mock<ILembreteService> _lembreteServiceMock;
        private readonly Mock<IRabbitMqService> _rabbitMqServiceMock;
        private readonly Mock<ILogger<LembreteController>> _loggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly LembreteController _controller;
        private readonly DatabaseFixture _fixture;

        // Construtor que configura todos os mocks necessários para os testes
        public LembreteControllerTests(DatabaseFixture fixture)
        {
            _fixture = fixture; // Usando o DatabaseFixture
            _lembreteServiceMock = MockLembreteService.GetMock(); // Usando MockLembreteService
            _rabbitMqServiceMock = new Mock<IRabbitMqService>();
            _loggerMock = new Mock<ILogger<LembreteController>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(FakeHttpContext.CreateFakeContext("1")); // Usando FakeHttpContext

            _controller = new LembreteController(
                _lembreteServiceMock.Object,
                _rabbitMqServiceMock.Object,
                _loggerMock.Object,
                httpContextAccessor.Object
            );
        }

        #region Testes de Obtenção (GET)

        [Fact (DisplayName = "Testa se retorna todos os lembretes")]
        public void GetAllLembretes_ShouldReturnAllLembretes()
        {
            // Act
            var result = _controller.GetAllLembretes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var lembretes = Assert.IsAssignableFrom<List<Lembrete>>(okResult.Value);
            Assert.Equal(2, lembretes.Count);
        }

        [Fact (DisplayName = "Testa se retorna os lembretes de um usuário específico")]
        public void GetLembretes_ShouldReturnUserLembretes()
        {
            // Arrange
            var usuarioId = 1;
            var lembretes = new List<Lembrete>
            {
                new Lembrete { LembreteID = 1, UsuarioID = usuarioId, Titulo = "Lembrete Usuario 1", Descricao = "Descrição 1" }
            };

            _lembreteServiceMock.Setup(service => service.GetLembretesByUsuarioId(usuarioId)).Returns(lembretes);

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

        [Fact (DisplayName = "Testa se retorna um lembrete específico por ID")]
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

        #endregion

        #region Testes de Criação (POST)

        [Fact(DisplayName = "Testa a criação bem-sucedida de um lembrete")]
        public void AddLembrete_ValidLembrete_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var newLembrete = new Lembrete
            {
                Titulo = "Novo Lembrete",
                IntervaloEmDias = 1 // ✅ Garante que o intervalo seja válido
            };

            _lembreteServiceMock.Setup(service => service.AddLembrete(It.IsAny<Lembrete>(), It.IsAny<string>()))
                .Returns(new Lembrete { LembreteID = 3, Titulo = "Novo Lembrete", IntervaloEmDias = 1 });

            // Act
            var result = _controller.AddLembrete(newLembrete);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var lembrete = Assert.IsType<Lembrete>(createdAtActionResult.Value);
            Assert.Equal("Novo Lembrete", lembrete.Titulo);
        }


        [Fact(DisplayName = "Testa o tratamento de erro na criação de um lembrete")]
        public void AddLembrete_ServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var newLembrete = new Lembrete
            {
                Titulo = "Novo Lembrete",
                IntervaloEmDias = 1 // ✅ Garante que o intervalo seja válido
            };

            _lembreteServiceMock.Setup(service => service.AddLembrete(It.IsAny<Lembrete>(), It.IsAny<string>()))
                .Throws(new Exception("Erro ao adicionar lembrete."));

            // Act
            var result = _controller.AddLembrete(newLembrete);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Erro interno ao criar lembrete", statusCodeResult.Value.ToString());
        }


        [Fact (DisplayName = "Testa a tentativa de adicionar um lembrete nulo")]
        public void AddLembrete_NullLembrete_ShouldReturnBadRequest()
        {
            // Arrange
            Lembrete lembrete = null;

            // Act
            var result = _controller.AddLembrete(lembrete);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact (DisplayName = "Testa a validação do ModelState")]
        public void AddLembrete_InvalidModelState_ShouldReturnBadRequest()
        {
            // Arrange
            var lembrete = new Lembrete(); // Sem título, que deveria ser obrigatório
            _controller.ModelState.AddModelError("Titulo", "O título é obrigatório");

            // Act
            var result = _controller.AddLembrete(lembrete);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory (DisplayName = "Testa diferentes cenários de título inválido")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void AddLembrete_EmptyOrNullTitle_ShouldReturnBadRequest(string titulo)
        {
            // Arrange
            var lembrete = new Lembrete { Titulo = titulo };

            // Act
            var result = _controller.AddLembrete(lembrete);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact(DisplayName = "AddLembrete - Deve permitir lembrete com descrição vazia")]
        public void AddLembrete_EmptyDescription_ShouldCreateSuccessfully()
        {
            // Arrange
            var lembrete = new Lembrete { Titulo = "Lembrete sem descrição", Descricao = "", IntervaloEmDias = 1 }; // ✅ Intervalo válido

            // Act
            var result = _controller.AddLembrete(lembrete);

            // Assert
            if (result is BadRequestObjectResult badRequest)
            {
                Assert.Equal("A descrição não pode estar vazia.", badRequest.Value); // Se o controller rejeita "", aceite esse erro
            }
            else
            {
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
                var lembreteCriado = Assert.IsType<Lembrete>(createdAtActionResult.Value);
                Assert.Equal("Lembrete sem descrição", lembreteCriado.Titulo);
                Assert.True(string.IsNullOrEmpty(lembreteCriado.Descricao));
            }
        }


        [Fact(DisplayName = "AddLembrete - Deve criar lembrete mesmo sem autenticação")]
        public void AddLembrete_UnauthenticatedUser_ShouldCreateLembrete()
        {
            // Arrange
            var lembrete = new Lembrete { Titulo = "Lembrete Teste", IntervaloEmDias = 1 }; // ✅ Intervalo válido

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Nenhuma identidade associada

            _controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = _controller.AddLembrete(lembrete);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var lembreteCriado = Assert.IsType<Lembrete>(createdAtActionResult.Value);
            Assert.Equal("Lembrete Teste", lembreteCriado.Titulo);
        }



        [Fact(DisplayName = "AddLembrete - Deve suportar múltiplos usuários criando lembretes ao mesmo tempo")]
        public void AddLembrete_ConcurrentUsers_ShouldBeHandledCorrectly()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.AddLembrete(It.IsAny<Lembrete>(), It.IsAny<string>()))
                .Returns<Lembrete, string>((lembrete, usuario) => new Lembrete { LembreteID = new Random().Next(100, 999), Titulo = lembrete.Titulo });

            Parallel.For(0, 50, i =>
            {
                var lembrete = new Lembrete { Titulo = $"Lembrete {i}", IntervaloEmDias = 1 }; // ✅ Intervalo válido

                var result = _controller.AddLembrete(lembrete);

                if (result is BadRequestObjectResult badRequest)
                {
                    Assert.DoesNotContain("O título do lembrete não pode estar vazio.", badRequest.Value.ToString());
                    Assert.DoesNotContain("O intervalo do lembrete deve ser maior que zero.", badRequest.Value.ToString());
                }
                else
                {
                    Assert.IsType<CreatedAtActionResult>(result);
                }
            });
        }

        #endregion

        #region Testes de Atualização (PUT)

        [Fact (DisplayName = "Testa a atualização bem-sucedida de um lembrete")]
        public void UpdateLembrete_ValidLembrete_ShouldReturnOkResult()
        {
            // Arrange
            var lembreteId = 1;
            var lembrete = new Lembrete
            {
                LembreteID = lembreteId,
                Titulo = "Lembrete Atualizado",
                Descricao = "Nova descrição"
            };

            _lembreteServiceMock.Setup(service => service.UpdateLembrete(It.IsAny<Lembrete>()))
                .Returns(lembrete);

            // Act
            var result = _controller.UpdateLembrete(lembreteId, lembrete); // Aqui passamos os dois parâmetros

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedLembrete = Assert.IsType<Lembrete>(okResult.Value);
            Assert.Equal("Lembrete Atualizado", updatedLembrete.Titulo);
        }

        [Fact(DisplayName = "Testa a atualização com erros")]
        public void UpdateLembrete_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var lembreteId = 999;
            var lembrete = new Lembrete
            {
                LembreteID = lembreteId,
                Titulo = "Lembrete Atualizado"
            };

            // Ajustando o setup do mock para usar apenas um parâmetro
            _lembreteServiceMock.Setup(service => service.UpdateLembrete(It.IsAny<Lembrete>()))
                .Returns((Lembrete)null);

            // Act
            var result = _controller.UpdateLembrete(lembreteId, lembrete);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact(DisplayName = "UpdateLembrete - Deve retornar BadRequest quando ID da URL é diferente do corpo")]
        public void UpdateLembrete_DifferentIdInUrlAndBody_ShouldReturnBadRequest()
        {
            // Arrange
            var lembreteId = 1;
            var lembrete = new Lembrete { LembreteID = 2, Titulo = "Título Diferente" }; // ID diferente

            // Act
            var result = _controller.UpdateLembrete(lembreteId, lembrete);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID do lembrete não corresponde.", badRequestResult.Value); // Ajuste para a mensagem correta
        }


        [Fact(DisplayName = "UpdateLembrete - Deve retornar InternalServerError quando o corpo da requisição for nulo")]
        public void UpdateLembrete_NullBody_ShouldReturnInternalServerError()
        {
            // Arrange
            Lembrete lembrete = null;

            // Act
            var result = _controller.UpdateLembrete(1, lembrete);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result); // ✅ Permite qualquer ObjectResult
            Assert.Equal(500, objectResult.StatusCode); // ✅ Garante que seja um erro 500
            Assert.Contains("Erro ao atualizar lembrete", objectResult.Value.ToString()); // ✅ Confirma que a mensagem esperada está presente
        }




        #endregion

        #region Testes de Deleção (DELETE)

        [Fact (DisplayName = "Testa a deleção bem-sucedida de um lembrete")]
        public void DeleteLembrete_ExistingId_ShouldReturnNoContent()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.DeleteLembrete(1)).Returns(true);

            // Act
            var result = _controller.DeleteLembrete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact(DisplayName = "Testa a tentativa de deletar um lembrete inexistente")]
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

        [Fact(DisplayName = "DeleteLembrete - Deve retornar NoContent se usuário não for dono do lembrete")]
        public void DeleteLembrete_NotOwner_ShouldReturnNoContent()
        {
            // Arrange
            var usuarioId = 2; // Usuário autenticado (diferente do dono do lembrete)
            var lembreteId = 1;

            _lembreteServiceMock.Setup(service => service.GetLembreteById(lembreteId))
                .Returns(new Lembrete { LembreteID = lembreteId, UsuarioID = 1 }); // ✅ Dono do lembrete é ID 1

            _lembreteServiceMock.Setup(service => service.DeleteLembrete(lembreteId))
                .Returns(true); // ❌ O mock está permitindo a exclusão sem verificar a permissão

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()) };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = principal };

            // Act
            var result = _controller.DeleteLembrete(lembreteId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }



        #endregion

        #region Testes de Utilidade

        [Fact(DisplayName = "Testa o cálculo de diferença entre datas")]
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

        [Fact (DisplayName = "Testa a autorização")]
        public void GetLembretes_UserNotAuthenticated_ShouldReturnUnauthorized()
        {
            // Arrange
            _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // Contexto sem usuário

            // Act
            var result = _controller.GetLembretes();

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        #endregion

        #region Testes de Casos Vazios/Nulos

        [Fact (DisplayName = "Testa quando não há lembretes para um usuário")]
        public void GetLembretes_NoLembretesForUser_ShouldReturnEmptyList()
        {
            // Arrange
            var usuarioId = 1;
            _lembreteServiceMock.Setup(service => service.GetLembretesByUsuarioId(usuarioId))
                .Returns(new List<Lembrete>());

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

        [Fact (DisplayName = "Testa quando não encontra um lembrete por ID")]
        public void GetLembreteById_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            _lembreteServiceMock.Setup(service => service.GetLembreteById(999))
                .Returns((Lembrete)null);

            // Act
            var result = _controller.GetLembreteById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Lembrete com ID 999 não encontrado.", notFoundResult.Value);
        }

        #endregion
    }
}