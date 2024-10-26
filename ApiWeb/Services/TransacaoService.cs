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
                return await _transacaoRepository.GetTransacoes();
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
                return await _transacaoRepository.GetTransacaoById(id)
                       ?? throw new KeyNotFoundException("Transação não encontrada.");
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
                if (transacaoDto.Tipo != "Entrada" && transacaoDto.Tipo != "Saida")
                {
                    throw new TipoTransicaoInvalida("Tipo de transação deve ser 'Entrada' ou 'Saida'.");
                }

                bool usuarioExiste = await _authRepository.UsuarioExiste(transacaoDto.UsuarioID);
                bool categoriaExiste = await _categoriaRepository.CategoriaExiste(transacaoDto.CategoriaID);

                if (!usuarioExiste || !categoriaExiste)
                {
                    throw new UsuarioCategoriaNaoExiste("Usuário ou Categoria não existe.");
                }

                await _transacaoRepository.AddTransacao(transacaoDto);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar transação: " + ex.Message, ex);
            }
        }

        public async Task UpdateTransacao(Transacao transacao)
        {
            try
            {
                await _transacaoRepository.UpdateTransacao(transacao);
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
                await _transacaoRepository.DeleteTransacao(id);
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
