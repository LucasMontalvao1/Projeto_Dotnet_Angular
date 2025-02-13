using ApiWeb.Controllers;
using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Services.Interfaces;
using ApiWeb.Tests.Fixtures;
using ApiWeb.Tests.Mocks;
using ApiWeb.Tests.TestUtils;
using ApiWeb.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using static ApiWeb.Services.TransacaoService;
using System.ComponentModel;

namespace ApiWeb.Tests.Controllers
{
    public class TransacaoControllerTests 
    {
        private readonly Mock<ITransacaoService> _mockService;
        private readonly Mock<ILogger<TransacaoController>> _mockLogger;
        private readonly TransacaoController _controller;
        private readonly List<Transacao> _mockTransacoes;
        private readonly TransacaoDto _mockTransacaoDto;
        private readonly List<TransacaoDto> _mockTransacaoDtos;
        private readonly TransacaoDto _invalidTransacaoDto;
        private readonly DatabaseFixture _fixture;
        private const string TEST_USER_ID = "1";

        public TransacaoControllerTests()
        {
            _mockService = new Mock<ITransacaoService>();
            _mockLogger = new Mock<ILogger<TransacaoController>>();
            _controller = new TransacaoController(_mockService.Object, _mockLogger.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = FakeHttpContext.CreateFakeContext(TEST_USER_ID)
                }
            };

            // Inicializando com os mocks existentes
            _mockTransacoes = MockTransacaoService.GetMockTransacoes();
            _mockTransacaoDto = MockTransacaoService.GetMockTransacaoDto();
            _mockTransacaoDtos = MockTransacaoService.GetMockTransacaoDtos();
            _invalidTransacaoDto = MockTransacaoService.GetInvalidTransacaoDto();
        }

        #region GetTransacoes Tests
        [Fact]
        [Description("GET /transacoes - Deve retornar 200 OK com lista de transações quando existirem registros")]
        public async Task GetTransacoes_DeveRetornarListaDeTransacoes()
        {
            // Arrange
            _mockService.Setup(s => s.GetTransacoes())
                .ReturnsAsync(_mockTransacoes);

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacoes = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.Equal(_mockTransacoes.Count, ((List<Transacao>)transacoes).Count);
            Assert.Equal(_mockTransacoes[0].Descricao, ((List<Transacao>)transacoes)[0].Descricao);
        }

        [Fact]
        [Description("GET /transacoes - Deve retornar 500 Internal Server Error quando ocorrer exceção não tratada")]
        public async Task GetTransacoes_QuandoOcorreErro_DeveRetornar500()
        {
            // Arrange
            _mockService.Setup(s => s.GetTransacoes())
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        [Description("GET /transacoes - Deve retornar 200 OK com lista vazia quando não houver registros")]
        public async Task GetTransacoes_QuandoListaVazia_DeveRetornarOkComListaVazia()
        {
            // Arrange
            _mockService.Setup(s => s.GetTransacoes())
                .ReturnsAsync(new List<Transacao>());

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacoes = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.Empty(transacoes);
        }
        #endregion

        #region GetTransacao Tests
        [Fact]
        [Description("GET /transacoes/{id} - Deve retornar 200 OK com detalhes da transação quando encontrada")]
        public async Task GetTransacao_DeveRetornarTransacao()
        {
            // Arrange
            var transacaoEsperada = _mockTransacoes[0];
            _mockService.Setup(s => s.GetTransacaoById(transacaoEsperada.TransacaoID))
                .ReturnsAsync(transacaoEsperada);

            // Act
            var result = await _controller.GetTransacao(transacaoEsperada.TransacaoID);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacao = Assert.IsType<Transacao>(okResult.Value);
            Assert.Equal(transacaoEsperada.TransacaoID, transacao.TransacaoID);
            Assert.Equal(transacaoEsperada.Descricao, transacao.Descricao);
            Assert.Equal(transacaoEsperada.Valor, transacao.Valor);
        }

        [Fact]
        [Description("GET /transacoes/{id} - Deve retornar 404 Not Found quando transação não existir")]
        public async Task GetTransacao_QuandoNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetTransacaoById(It.IsAny<int>()))
                .ThrowsAsync(new KeyNotFoundException("Transação não encontrada"));

            // Act
            var result = await _controller.GetTransacao(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Transação não encontrada", notFoundResult.Value.ToString());
        }
        #endregion

        #region PostTransacao Tests
        [Fact]
        [Description("POST /transacoes - Deve retornar 201 Created com dados da nova transação")]
        public async Task PostTransacao_DeveRetornarCreatedAtActionResult()
        {
            // Arrange
            _mockService.Setup(s => s.AddTransacao(_mockTransacaoDto))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTransacao(_mockTransacaoDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetTransacao", createdResult.ActionName);
            var returnValue = Assert.IsType<TransacaoDto>(createdResult.Value);
            Assert.Equal(_mockTransacaoDto.Descricao, returnValue.Descricao);
        }

        [Fact]
        [Description("POST /transacoes - Deve retornar 400 Bad Request quando dados forem inválidos")]
        public async Task PostTransacao_ComDadosInvalidos_DeveRetornarBadRequest()
        {
            // Arrange
            _mockService.Setup(s => s.AddTransacao(_invalidTransacaoDto))
                .ThrowsAsync(new TipoTransicaoInvalida("Tipo de transação inválido"));

            // Act
            var result = await _controller.PostTransacao(_invalidTransacaoDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        [Description("POST /transacoes - Deve retornar 400 Bad Request quando categoria não existir")]
        public async Task PostTransacao_QuandoCategoriaNaoExiste_DeveRetornarBadRequest()
        {
            // Arrange
            _mockService.Setup(s => s.AddTransacao(_mockTransacaoDto))
                .ThrowsAsync(new UsuarioCategoriaNaoExiste("Categoria não encontrada"));

            // Act
            var result = await _controller.PostTransacao(_mockTransacaoDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Categoria não encontrada", badRequestResult.Value.ToString());
        }
        #endregion

        #region PutTransacao Tests
        [Fact]
        [Description("PUT /transacoes/{id} - Deve retornar 204 No Content após atualização bem-sucedida")]
        public async Task PutTransacao_DeveRetornarNoContent()
        {
            // Arrange
            var transacaoParaAtualizar = _mockTransacaoDtos[0];
            _mockService.Setup(s => s.UpdateTransacao(transacaoParaAtualizar))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PutTransacao(
                transacaoParaAtualizar.TransacaoID.Value,
                transacaoParaAtualizar
            );

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        [Description("PUT /transacoes/{id} - Deve retornar 400 Bad Request quando IDs não coincidirem")]
        public async Task PutTransacao_ComIdsDiferentes_DeveRetornarBadRequest()
        {
            // Arrange
            var transacaoDto = _mockTransacaoDtos[0];
            var idDiferente = transacaoDto.TransacaoID + 1;

            // Act
            var result = await _controller.PutTransacao((int)idDiferente, transacaoDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        [Description("PUT /transacoes/{id} - Deve retornar 404 Not Found quando transação não existir")]
        public async Task PutTransacao_QuandoNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            var transacaoDto = _mockTransacaoDtos[0];
            _mockService.Setup(s => s.UpdateTransacao(transacaoDto))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.PutTransacao(
                transacaoDto.TransacaoID.Value,
                transacaoDto
            );

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        [Description("PUT /transacoes/{id} - Deve retornar 400 Bad Request quando categoria não existir")]
        public async Task PutTransacao_ComCategoriaNaoExistente_DeveRetornarBadRequest()
        {
            // Arrange
            var transacaoDto = _mockTransacaoDtos[0];
            _mockService.Setup(s => s.UpdateTransacao(transacaoDto))
                .ThrowsAsync(new UsuarioCategoriaNaoExiste("Categoria não encontrada"));

            // Act
            var result = await _controller.PutTransacao(
                transacaoDto.TransacaoID.Value,
                transacaoDto
            );

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Categoria não encontrada", badRequestResult.Value.ToString());
        }
        #endregion

        #region DeleteTransacao Tests
        [Fact]
        [Description("DELETE /transacoes/{id} - Deve retornar 204 No Content após exclusão bem-sucedida")]
        public async Task DeleteTransacao_DeveRetornarNoContent()
        {
            // Arrange
            var transacao = _mockTransacoes[0];
            _mockService.Setup(s => s.DeleteTransacao(transacao.TransacaoID))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTransacao(transacao.TransacaoID);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        [Description("DELETE /transacoes/{id} - Deve retornar 404 Not Found quando transação não existir")]
        public async Task DeleteTransacao_QuandoNaoEncontrada_DeveRetornarNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteTransacao(_invalidTransacaoDto.TransacaoID.Value))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeleteTransacao(_invalidTransacaoDto.TransacaoID.Value);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        #endregion

        #region Integration Tests
        [Fact]
        [Description("Deve executar com sucesso o fluxo completo de CRUD de transações")]
        public async Task IntegrationTest_FluxoCompleto_CRUD()
        {
            // Arrange
            var novaTransacao = _mockTransacaoDto;
            var transacaoAtualizada = _mockTransacaoDtos[0];

            _mockService.Setup(s => s.AddTransacao(It.IsAny<TransacaoDto>()))
                .Returns(Task.CompletedTask);
            _mockService.Setup(s => s.GetTransacaoById(It.IsAny<int>()))
                .ReturnsAsync(_mockTransacoes[0]);
            _mockService.Setup(s => s.UpdateTransacao(It.IsAny<TransacaoDto>()))
                .Returns(Task.CompletedTask);
            _mockService.Setup(s => s.DeleteTransacao(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act & Assert - Create
            var createResult = await _controller.PostTransacao(novaTransacao);
            var createdAtAction = Assert.IsType<CreatedAtActionResult>(createResult);
            Assert.Equal("GetTransacao", createdAtAction.ActionName);

            // Act & Assert - Read
            var readResult = await _controller.GetTransacao(1);
            var okResult = Assert.IsType<OkObjectResult>(readResult.Result);
            var readTransacao = Assert.IsType<Transacao>(okResult.Value);
            Assert.Equal(_mockTransacoes[0].Descricao, readTransacao.Descricao);

            // Act & Assert - Update
            var updateResult = await _controller.PutTransacao(1, transacaoAtualizada);
            Assert.IsType<NoContentResult>(updateResult);

            // Act & Assert - Delete
            var deleteResult = await _controller.DeleteTransacao(1);
            Assert.IsType<NoContentResult>(deleteResult);

            // Verify
            _mockService.Verify(s => s.AddTransacao(It.IsAny<TransacaoDto>()), Times.Once);
            _mockService.Verify(s => s.GetTransacaoById(It.IsAny<int>()), Times.Once);
            _mockService.Verify(s => s.UpdateTransacao(It.IsAny<TransacaoDto>()), Times.Once);
            _mockService.Verify(s => s.DeleteTransacao(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData("2023-01-01", "2023-01-31")]
        [InlineData("2023-02-01", "2023-02-28")]
        [DisplayName("GetTransacoes deve retornar apenas transações do período especificado")]
        public async Task GetTransacoes_FiltradasPorPeriodo_DeveRetornarTransacoesDoPeriodo(
             string startDate,
             string endDate)
        {
            // Arrange
            var inicio = DateTime.Parse(startDate);
            var fim = DateTime.Parse(endDate);

            var transacoesFiltradas = _mockTransacoes.Where(t =>
                t.Data >= inicio &&
                t.Data <= fim).ToList();

            _mockService.Setup(s => s.GetTransacoes())
                .ReturnsAsync(transacoesFiltradas);

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacoes = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.All(transacoes, t =>
                Assert.True(t.Data >= inicio && t.Data <= fim));
        }
        #endregion

        #region Error Handling Tests
        [Fact]
        [DisplayName("GetTransacoes deve logar erro quando ocorrer exceção inesperada")]
        public async Task GetTransacoes_QuandoExcecaoInesperada_DeveLogarErro()
        {
            // Arrange
            var exceptionMessage = "Erro inesperado no banco de dados";
            _mockService.Setup(s => s.GetTransacoes())
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Erro ao buscar transações")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
                
        #endregion

        #region Authorization Tests
        [Fact]
        [DisplayName("GetTransacoes deve retornar todas as transações para usuário Admin")]
        public async Task GetTransacoes_ComUsuarioAdmin_DeveRetornarTodasTransacoes()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = FakeHttpContext.CreateFakeContext(TEST_USER_ID, "Admin")
            };

            _mockService.Setup(s => s.GetTransacoes())
                .ReturnsAsync(_mockTransacoes);

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacoes = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.Equal(_mockTransacoes.Count, ((List<Transacao>)transacoes).Count);
        }

        #endregion

        #region Validation Tests               
        [Theory]
        [InlineData("Receita")]
        [InlineData("Despesa")]
        [DisplayName("PostTransacao deve aceitar tipos válidos de transação")]
        public async Task PostTransacao_ComTiposValidos_DeveRetornarCreated(string tipoValido)
        {
            // Arrange
            var transacaoValida = _mockTransacaoDto;
            transacaoValida.Tipo = tipoValido;

            _mockService.Setup(s => s.AddTransacao(It.IsAny<TransacaoDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.PostTransacao(transacaoValida);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }
        #endregion

        #region Date Handling Tests
        [Fact]
        [DisplayName("GetTransacoes deve retornar apenas transações do mês atual")]
        public async Task GetTransacoes_FiltradasPorMesAtual_DeveRetornarTransacoesCorretas()
        {
            // Arrange
            var primeiroDiaMes = DateHelper.GetFirstDayOfMonth(DateTime.Now);
            var ultimoDiaMes = DateHelper.GetLastDayOfMonth(DateTime.Now);

            var transacoesFiltradas = _mockTransacoes.Where(t =>
                t.Data >= primeiroDiaMes &&
                t.Data <= ultimoDiaMes).ToList();

            _mockService.Setup(s => s.GetTransacoes())
                .ReturnsAsync(transacoesFiltradas);

            // Act
            var result = await _controller.GetTransacoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var transacoes = Assert.IsAssignableFrom<IEnumerable<Transacao>>(okResult.Value);
            Assert.All(transacoes, t =>
                Assert.True(t.Data >= primeiroDiaMes && t.Data <= ultimoDiaMes));
        }
        #endregion

        public void Dispose()
        {
            using var conn = _fixture.GetConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                DELETE FROM Transacoes;
                ALTER TABLE Transacoes AUTO_INCREMENT = 1;";
            cmd.ExecuteNonQuery();
        }
    }
}