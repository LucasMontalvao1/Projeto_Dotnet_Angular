using AutoMapper;
using FluentValidation;
using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.DTOs.Responses;
using FinanceReminderHub.Application.Interfaces;
using FinanceReminderHub.Domain.Entities;
using FinanceReminderHub.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinanceReminderHub.Application.Services
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<TransacaoRequest> _validator;

        public TransacaoService(ITransacaoRepository repository, IMapper mapper, IValidator<TransacaoRequest> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<TransacaoResponse>> GetTransacoes()
        {
            var transacoes = await _repository.GetTransacoes();
            return _mapper.Map<IEnumerable<TransacaoResponse>>(transacoes);
        }

        public async Task<TransacaoResponse> GetTransacaoById(int id)
        {
            var transacao = await _repository.GetTransacaoById(id);
            return _mapper.Map<TransacaoResponse>(transacao);
        }

        public async Task AddTransacao(TransacaoRequest transacaoRequest)
        {
            var validationResult = await _validator.ValidateAsync(transacaoRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var transacao = _mapper.Map<Transacao>(transacaoRequest);
            await _repository.AddTransacao(transacao);
        }

        public async Task UpdateTransacao(int id, TransacaoRequest transacaoRequest)
        {
            var validationResult = await _validator.ValidateAsync(transacaoRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var transacao = _mapper.Map<Transacao>(transacaoRequest);
            transacao.TransacaoID = id;

            await _repository.UpdateTransacao(transacao);
        }

        public async Task DeleteTransacao(int id)
        {
            await _repository.DeleteTransacao(id);
        }
    }
}
