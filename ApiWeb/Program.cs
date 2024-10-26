using ApiLucas.Infra.Data;
using ApiWeb.Hubs;
using ApiWeb.Services.Hangfire;
using ApiWeb.Repositorys;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services;
using ApiWeb.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using Hangfire;
using Hangfire.MySql;
using System.Transactions;
using Serilog.Filters;


var builder = WebApplication.CreateBuilder(args);

// Configura��o do Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Hangfire", Serilog.Events.LogEventLevel.Warning) // Aumentar o n�vel de log do Hangfire
    .MinimumLevel.Information()
    .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics")) // Filtrar logs do ASP.NET
    .Filter.ByExcluding(logEvent => logEvent.Properties.ContainsKey("RequestPath") && logEvent.Properties["RequestPath"].ToString().Contains("/hangfire/stats")) // Filtrar requisi��es do Hangfire
    .WriteTo.Console()
    .WriteTo.File("Logs/myapp-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();
;

builder.Host.UseSerilog();

// Adicionar controladores
builder.Services.AddControllers();

// Adicionar servi�os do SignalR
builder.Services.AddSignalR();

// Configurar o Swagger para documentar a API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Lucas", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Por favor, insira o token: Bearer {token}"
    });
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

builder.Services.AddHttpContextAccessor();

// Configura��o do banco de dados e reposit�rios
builder.Services.AddSingleton<MySqlConnectionDB>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddScoped<IHangfireService, HangfireService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ILembreteRepository, LembreteRepository>();
builder.Services.AddScoped<ILembreteService, LembreteService>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();


// Configura��o de pol�tica de CORS para permitir todas as origens
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configura��o do Hangfire
var connectionString = "server=localhost;userid=root;password=asd123; Allow User Variables=True;database=projeto_producao ";

builder.Services.AddHangfire(configuration =>
{
    var options = new MySqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(10), // Checa a fila a cada 10 segundos
        TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        
    };

    configuration.UseStorage(new MySqlStorage(connectionString, options));
});

builder.Services.AddHangfireServer(options =>
{
    options.ServerCheckInterval = TimeSpan.FromMinutes(10); // Checa novos jobs a cada 10 minutos
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
        ValidateLifetime = true
    };
});

builder.Logging.ClearProviders();
builder.Host.UseSerilog();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddFilter("Microsoft.AspNetCore.Routing.HttpRoutingMiddleware", LogLevel.Error); // Ignorar logs do Hangfire


// Construir a aplica��o
var app = builder.Build();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobs.AddOrUpdate<HangfireService>(
    "VerificarLembretesRepetidos",   // Nome do job
    x => x.ExecuteJob(),              // M�todo a ser chamado
    Cron.Hourly                        // Executa de hora em hora
);

// Mapeie o Hub
app.MapHub<LembreteHub>("/lembreteHub");

// Configura��o para o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    Log.Information("Iniciando em modo de desenvolvimento");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Lucas v1");
        c.RoutePrefix = "swagger";
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

// Permite o acesso a arquivos est�ticos
app.UseStaticFiles();

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
