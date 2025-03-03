using ApiWeb.Controllers;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ApiWeb.Tests.Controllers
{
    public class HangfireControllerTests
    {
        private readonly Mock<IHangfireService> _mockHangfireService;
        private readonly Mock<ILogger<HangfireController>> _mockLogger;
        private readonly HangfireController _controller;

        public HangfireControllerTests()
        {
            _mockHangfireService = new Mock<IHangfireService>();
            _mockLogger = new Mock<ILogger<HangfireController>>();
            _controller = new HangfireController(_mockHangfireService.Object, _mockLogger.Object);
        }

        [Fact]
        [DisplayName("DispararJob deve retornar Ok quando o job é agendado com sucesso")]
        public void DispararJob_RetornaOk_QuandoJobAgendadoComSucesso()
        {
            // Arrange
            _mockHangfireService.Setup(service => service.EnqueueJob());

            // Act
            var resultado = _controller.DispararJob();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var value = Assert.IsAssignableFrom<object>(okResult.Value);

            // Usando reflexão para acessar a propriedade "Message" do objeto anônimo
            var messageProperty = value.GetType().GetProperty("Message");
            var messageValue = messageProperty.GetValue(value).ToString();

            Assert.Equal("Job 'VerificarLembretesRepetidos' disparada com sucesso!", messageValue);
        }

        [Fact]
        [DisplayName("DispararJob deve verificar se o serviço EnqueueJob foi chamado")]
        public void DispararJob_DeveVerificarChamadaAoServico()
        {
            // Arrange
            _mockHangfireService.Setup(service => service.EnqueueJob());

            // Act
            _controller.DispararJob();

            // Assert
            _mockHangfireService.Verify(service => service.EnqueueJob(), Times.Once);
        }

        [Fact]
        [DisplayName("DispararJob deve verificar se o logger registra informação de sucesso")]
        public void DispararJob_DeveVerificarLoggerInformacao()
        {
            // Arrange
            _mockHangfireService.Setup(service => service.EnqueueJob());

            // Act
            _controller.DispararJob();

            // Assert
            // Verificar se o logger foi chamado com LogInformation e a mensagem esperada
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Job 'VerificarLembretesRepetidos' disparada com sucesso")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [DisplayName("DispararJob deve retornar StatusCode 500 quando ocorre uma exceção")]
        public void DispararJob_RetornaStatusCode500_QuandoOcorreExcecao()
        {
            // Arrange
            _mockHangfireService.Setup(service => service.EnqueueJob())
                .Throws(new Exception("Erro simulado"));

            // Act
            var resultado = _controller.DispararJob();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(resultado);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var value = Assert.IsAssignableFrom<object>(statusCodeResult.Value);
            var messageProperty = value.GetType().GetProperty("Message");
            var messageValue = messageProperty.GetValue(value).ToString();

            Assert.Equal("Ocorreu um erro ao disparar o job.", messageValue);
        }

        [Fact]
        [DisplayName("DispararJob deve verificar se o logger registra erro quando ocorre exceção")]
        public void DispararJob_DeveVerificarLoggerErro_QuandoOcorreExcecao()
        {
            // Arrange
            var excecaoSimulada = new Exception("Erro simulado");
            _mockHangfireService.Setup(service => service.EnqueueJob())
                .Throws(excecaoSimulada);

            // Act
            _controller.DispararJob();

            // Assert
            // Verificar se o logger foi chamado com LogError e a mensagem esperada
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Erro ao disparar o job")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        [DisplayName("DispararJob deve garantir que a exceção é tratada e não propagada")]
        public void DispararJob_GaranteQueExcecaoETratada()
        {
            // Arrange
            _mockHangfireService.Setup(service => service.EnqueueJob())
                .Throws(new Exception("Erro simulado"));

            // Act & Assert - Não deve lançar exceção
            var resultado = _controller.DispararJob();
            Assert.NotNull(resultado);
        }
    }
}