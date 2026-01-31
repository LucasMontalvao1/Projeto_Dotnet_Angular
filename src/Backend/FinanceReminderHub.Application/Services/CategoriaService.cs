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
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CategoriaRequest> _validator;

        public CategoriaService(ICategoriaRepository repository, IMapper mapper, IValidator<CategoriaRequest> validator)
        {
            _repository = repository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<CategoriaResponse>> GetCategorias()
        {
            var categorias = await _repository.GetCategorias();
            return _mapper.Map<IEnumerable<CategoriaResponse>>(categorias);
        }

        public async Task<CategoriaResponse> GetCategoriaById(int id)
        {
            var categoria = await _repository.GetCategoriaById(id);
            return _mapper.Map<CategoriaResponse>(categoria);
        }

        public async Task<int> AddCategoria(CategoriaRequest categoriaRequest)
        {
            var validationResult = await _validator.ValidateAsync(categoriaRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var categoria = _mapper.Map<Categoria>(categoriaRequest);
            return await _repository.AddCategoria(categoria);
        }

        public async Task UpdateCategoria(int id, CategoriaRequest categoriaRequest)
        {
            var validationResult = await _validator.ValidateAsync(categoriaRequest);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var categoria = _mapper.Map<Categoria>(categoriaRequest);
            categoria.CategoriaID = id;

            await _repository.UpdateCategoria(categoria);
        }

        public async Task DeleteCategoria(int id)
        {
            await _repository.DeleteCategoria(id);
        }
    }
}
