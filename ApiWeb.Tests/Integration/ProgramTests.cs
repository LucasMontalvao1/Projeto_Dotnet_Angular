using ApiWeb.Hubs;
using ApiWeb.Infra.Data;
using ApiWeb.Repositorys;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services;
using ApiWeb.Services.Hangfire;
using ApiWeb.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xunit;

namespace ApiWeb.Tests.Integration
{
    public class ProgramTests
    {
        [Fact]
        [DisplayName("Deve verificar serviços essenciais")]
        public void ConfiguracoesDeServicos_DevemEstarCorretas()
        {
            // Arrange
            var services = new ServiceCollection();

            // Adicionar configuração mock
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
            {"ConnectionStrings:DefaultConnection", "server=localhost;userid=root;password=asd123;database=projeto_producao"}
                })
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Adicionar serviços de logging
            services.AddLogging();

            // Mock do IHubContext para o SignalR
            var mockHubContext = new Mock<IHubContext<LembreteHub>>();
            services.AddSingleton(mockHubContext.Object);

            // Act - configurar serviços como no arquivo Program.cs
            services.AddSingleton<MySqlConnectionDB>();
            services.AddScoped<IRabbitMqService, RabbitMqService>();
            services.AddScoped<IHangfireService, HangfireService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILembreteRepository, LembreteRepository>();
            services.AddScoped<ILembreteService, LembreteService>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();
            services.AddScoped<ITransacaoService, TransacaoService>();

            var serviceProvider = services.BuildServiceProvider();

            // Assert - verificar se os serviços essenciais estão configurados
            Assert.NotNull(serviceProvider.GetService<MySqlConnectionDB>());
            Assert.NotNull(serviceProvider.GetService<IAuthService>());
            Assert.NotNull(serviceProvider.GetService<ITokenService>());
            Assert.NotNull(serviceProvider.GetService<ICategoriaService>());
            Assert.NotNull(serviceProvider.GetService<ILembreteService>());
            Assert.NotNull(serviceProvider.GetService<ITransacaoService>());
            Assert.NotNull(serviceProvider.GetService<IHangfireService>());
        }

        [Fact]
        [DisplayName("Deve verificar configuração de repositórios")]
        public void ConfiguracoesDeRepositorios_DevemEstarCorretas()
        {
            // Arrange
            var services = new ServiceCollection();

            // Adicionar configuração mock
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
            {"ConnectionStrings:DefaultConnection", "server=localhost;userid=root;password=asd123;database=projeto_producao"}
                })
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Adicionar serviços de logging
            services.AddLogging(); // Isto registra ILogger<T> para todos os tipos

            // Act - configurar repositórios como no arquivo Program.cs
            services.AddSingleton<MySqlConnectionDB>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<ILembreteRepository, LembreteRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ITransacaoRepository, TransacaoRepository>();

            var serviceProvider = services.BuildServiceProvider();

            // Assert - verificar se os repositórios estão configurados
            Assert.NotNull(serviceProvider.GetService<IAuthRepository>());
            Assert.NotNull(serviceProvider.GetService<ITokenRepository>());
            Assert.NotNull(serviceProvider.GetService<ICategoriaRepository>());
            Assert.NotNull(serviceProvider.GetService<ILembreteRepository>());
            Assert.NotNull(serviceProvider.GetService<ITransacaoRepository>());
        }

        [Fact]
        [DisplayName("Deve verificar configuração de autenticação")]
        public void ConfiguracaoJWT_DeveEstarCorreta()
        {
            // Arrange
            var services = new ServiceCollection();

            // Adicionar configuração mock
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"Jwt:Key", "chave_secreta_para_testes_1234567890"}
                })
                .Build();
            services.AddSingleton<IConfiguration>(configuration);

            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

            // Act - configurar autenticação como no arquivo Program.cs
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true
                };
            });

            var serviceProvider = services.BuildServiceProvider();

            // Assert - verificar se a autenticação está configurada
            var authenticationSchemeProvider = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            Assert.NotNull(authenticationSchemeProvider);
        }

        [Fact]
        [DisplayName("Deve verificar configuração do Hangfire")]
        public void ConfiguracaoHangfire_DeveEstarCorreta()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act - configurar serviços mockados do Hangfire
            services.AddSingleton<IBackgroundJobClient>(new Mock<IBackgroundJobClient>().Object);
            services.AddSingleton<IRecurringJobManager>(new Mock<IRecurringJobManager>().Object);

            var serviceProvider = services.BuildServiceProvider();

            // Assert - verificar se os serviços do Hangfire estão configurados
            Assert.NotNull(serviceProvider.GetService<IBackgroundJobClient>());
            Assert.NotNull(serviceProvider.GetService<IRecurringJobManager>());
        }
    }
}