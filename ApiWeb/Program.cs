using ApiLucas.Infra.Data;
using ApiWeb.Repositorys;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/myapp-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext() 
    .CreateLogger();

// Adicionar controladores
builder.Services.AddControllers();

// Configurar o Swagger para documentar a API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Lucas", Version = "v1" });

    // Configura��o de seguran�a do Swagger para aceitar JWT tokens
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter your JWT token in the format: Bearer {token}"
    });

    // Definir a necessidade de incluir o token para todas as requisi��es
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configura��o do banco de dados e reposit�rios
builder.Services.AddSingleton<MySqlConnectionDB>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ILembreteRepository, LembreteRepository>();
builder.Services.AddScoped<ILembreteService, LembreteService>();

// Configura��o de pol�tica de CORS para permitir todas as origens
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configura��o da chave JWT a partir do appsettings.json ou Configuration
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

// Configura��o da autentica��o JWT
builder.Services.AddAuthentication(options =>
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
        ValidateLifetime = true // Valida o tempo de expira��o do token
    };
});

builder.Logging.ClearProviders(); 
builder.Host.UseSerilog(); 
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 
builder.Logging.AddDebug(); 

// Construir a aplica��o
var app = builder.Build();

// Configura��o para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    Log.Information("Iniciando em modo de desenvolvimento");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Lucas v1");
        c.RoutePrefix = string.Empty; 
    });
}
else
{
    Log.Information("Iniciando em modo de produ��o");
}

// Habilitar HTTPS redirection
app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowAll");

// Configurar autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

// Mapear os controladores
app.MapControllers();

// Rodar a aplica��o
Log.Information("Aplica��o iniciada com sucesso");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplica��o falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush(); // Fecha e descarrega o logger no final da aplica��o
}