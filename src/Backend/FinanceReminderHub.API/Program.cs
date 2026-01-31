using FinanceReminderHub.Application.Interfaces;
using FinanceReminderHub.Application.Mappings;
using FinanceReminderHub.Application.Services;
using FinanceReminderHub.Application.Validators;
using FinanceReminderHub.Domain.Interfaces;
using FinanceReminderHub.Infrastructure.Context;
using FinanceReminderHub.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Dapper Context
builder.Services.AddSingleton<DapperContext>();

// 2. Repositories
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ILembreteRepository, LembreteRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

// 3. Services
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ILembreteService, LembreteService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 5. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CategoriaRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
