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
    public class LembreteService : ILembreteService
    {
        private readonly ILembreteRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<LembreteRequest> _validator;

        public LembreteService(ILembreteRepository repository, IMapper mapper, IValidator<LembreteRequest> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<LembreteResponse>> GetLembretesByUsuarioId(int usuarioId)
        {
            var lembretes = await _repository.GetLembretesByUsuarioId(usuarioId);
            return _mapper.Map<IEnumerable<LembreteResponse>>(lembretes);
        }

        public async Task<LembreteResponse> GetLembreteById(int id)
        {
            var lembrete = await _repository.GetLembreteById(id);
            return _mapper.Map<LembreteResponse>(lembrete);
        }

        public async Task<int> AddLembrete(LembreteRequest lembreteRequest)
        {
            var validationResult = await _validator.ValidateAsync(lembreteRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var lembrete = _mapper.Map<Lembrete>(lembreteRequest);
            return await _repository.AddLembrete(lembrete);
        }

        public async Task UpdateLembrete(int id, LembreteRequest lembreteRequest)
        {
            var validationResult = await _validator.ValidateAsync(lembreteRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var lembrete = _mapper.Map<Lembrete>(lembreteRequest);
            lembrete.LembreteID = id;

            await _repository.UpdateLembrete(lembrete);
        }

        public async Task DeleteLembrete(int id)
        {
            await _repository.DeleteLembrete(id);
        }
    }
}
