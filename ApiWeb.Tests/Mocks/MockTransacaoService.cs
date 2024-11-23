using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using System;
using System.Collections.Generic;

namespace ApiWeb.Tests.Mocks
{
    public static class MockTransacaoService
    {
        public static List<Transacao> GetMockTransacoes()
        {
            return new List<Transacao>
            {
                new Transacao
                {
                    TransacaoID = 1,
                    UsuarioID = 1,
                    CategoriaID = 1,
                    Descricao = "Salário",
                    Valor = 5000.00M,
                    Data = DateTime.Now.AddDays(-5),
                    Tipo = "Receita"
                },
                new Transacao
                {
                    TransacaoID = 2,
                    UsuarioID = 1,
                    CategoriaID = 2,
                    Descricao = "Aluguel",
                    Valor = 1500.00M,
                    Data = DateTime.Now.AddDays(-3),
                    Tipo = "Despesa"
                },
                new Transacao
                {
                    TransacaoID = 3,
                    UsuarioID = 1,
                    CategoriaID = 3,
                    Descricao = "Freelance",
                    Valor = 2000.00M,
                    Data = DateTime.Now.AddDays(-1),
                    Tipo = "Receita"
                }
            };
        }

        public static TransacaoDto GetMockTransacaoDto()
        {
            return new TransacaoDto
            {
                TransacaoID = 1,
                UsuarioID = 1,
                CategoriaID = 1,
                Descricao = "Nova Transação",
                Valor = 1000.00M,
                Data = DateTime.Now,
                Tipo = "Receita"
            };
        }

        public static List<TransacaoDto> GetMockTransacaoDtos()
        {
            return new List<TransacaoDto>
            {
                new TransacaoDto
                {
                    TransacaoID = 1,
                    UsuarioID = 1,
                    CategoriaID = 1,
                    Descricao = "Salário",
                    Valor = 5000.00M,
                    Data = DateTime.Now.AddDays(-5),
                    Tipo = "Receita"
                },
                new TransacaoDto
                {
                    TransacaoID = 2,
                    UsuarioID = 1,
                    CategoriaID = 2,
                    Descricao = "Aluguel",
                    Valor = 1500.00M,
                    Data = DateTime.Now.AddDays(-3),
                    Tipo = "Despesa"
                }
            };
        }

        public static TransacaoDto GetInvalidTransacaoDto()
        {
            return new TransacaoDto
            {
                TransacaoID = 999,
                UsuarioID = 999,
                CategoriaID = 999,
                Descricao = "Transação Inválida",
                Valor = -1,
                Data = DateTime.Now,
                Tipo = "TipoInvalido"
            };
        }
    }
}