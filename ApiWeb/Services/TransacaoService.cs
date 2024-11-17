using ApiWeb.Models;
using ApiWeb.Models.DTOs;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiWeb.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IAuthRepository _authRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository, IAuthRepository authRepository, ICategoriaRepository categoriaRepository)
        {
            _transacaoRepository = transacaoRepository;
            _authRepository = authRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<IEnumerable<Transacao>> GetTransacoes()
        {
            try
            {
                var transacoes = await _transacaoRepository.GetTransacoes();
                foreach (var transacao in transacoes)
                {
                    // Carregar os dados completos da categoria para cada transação
                    var categoria = await _categoriaRepository.GetCategoriaById(transacao.CategoriaID);
                    transacao.Categoria = categoria;
                }
                return transacoes;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter transações: " + ex.Message, ex);
            }
        }

        public async Task<Transacao> GetTransacaoById(int id)
        {
            try
            {
                var transacao = await _transacaoRepository.GetTransacaoById(id);
                if (transacao == null)
                    throw new KeyNotFoundException("Transação não encontrada.");

                // Carregar os dados completos da categoria
                var categoria = await _categoriaRepository.GetCategoriaById(transacao.CategoriaID);
                transacao.Categoria = categoria;

                return transacao;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter transação pelo ID: " + ex.Message, ex);
            }
        }

        public async Task AddTransacao(TransacaoDto transacaoDto)
        {
            try
            {
                if (transacaoDto.Tipo != "Entrada" && transacaoDto.Tipo != "Saída")
                {
                    throw new TipoTransicaoInvalida("Tipo de transação deve ser 'Entrada' ou 'Saída'.");
                }

                bool usuarioExiste = await _authRepository.UsuarioExiste(transacaoDto.UsuarioID);
                bool categoriaExiste = await _categoriaRepository.CategoriaExiste(transacaoDto.CategoriaID);

                if (!usuarioExiste)
                {
                    throw new UsuarioCategoriaNaoExiste("Usuário não encontrado.");
                }

                if (!categoriaExiste)
                {
                    throw new UsuarioCategoriaNaoExiste("Categoria não encontrada.");
                }

                await _transacaoRepository.AddTransacao(transacaoDto);
            }
            catch (TipoTransicaoInvalida)
            {
                throw;
            }
            catch (UsuarioCategoriaNaoExiste)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar transação: " + ex.Message, ex);
            }
        }

        public async Task UpdateTransacao(TransacaoDto transacaoDto)
        {
            try
            {
                // Verificar se a categoria existe
                bool categoriaExiste = await _categoriaRepository.CategoriaExiste(transacaoDto.CategoriaID);
                if (!categoriaExiste)
                {
                    throw new UsuarioCategoriaNaoExiste("Categoria não encontrada.");
                }

                // Atualizar os dados da categoria
                var categoria = await _categoriaRepository.GetCategoriaById(transacaoDto.CategoriaID);

                // Criar objeto Transacao a partir do DTO
                var transacao = new Transacao
                {
                    TransacaoID = transacaoDto.TransacaoID ?? 0, 
                    UsuarioID = transacaoDto.UsuarioID,
                    CategoriaID = transacaoDto.CategoriaID,
                    Tipo = transacaoDto.Tipo,
                    Valor = transacaoDto.Valor,
                    Descricao = transacaoDto.Descricao,
                    Data = transacaoDto.Data,
                    Categoria = categoria
                };

                await _transacaoRepository.UpdateTransacao(transacao);
            }
            catch (UsuarioCategoriaNaoExiste)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar transação: " + ex.Message, ex);
            }
        }

        public async Task DeleteTransacao(int id)
        {
            try
            {
                // Verificar se a transação existe antes de deletar
                var transacao = await _transacaoRepository.GetTransacaoById(id);
                if (transacao == null)
                {
                    throw new KeyNotFoundException("Transação não encontrada.");
                }

                await _transacaoRepository.DeleteTransacao(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar transação: " + ex.Message, ex);
            }
        }

        public class TipoTransicaoInvalida : ArgumentException
        {
            public TipoTransicaoInvalida(string message) : base(message) { }
        }

        public class UsuarioCategoriaNaoExiste : ArgumentException
        {
            public UsuarioCategoriaNaoExiste(string message) : base(message) { }
        }
    }
}