using AutoMapper;
using FinanceReminderHub.Application.DTOs.Requests;
using FinanceReminderHub.Application.DTOs.Responses;
using FinanceReminderHub.Domain.Entities;

namespace FinanceReminderHub.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Categoria
            CreateMap<CategoriaRequest, Categoria>();
            CreateMap<Categoria, CategoriaResponse>();

            // Lembrete
            CreateMap<LembreteRequest, Lembrete>();
            CreateMap<Lembrete, LembreteResponse>();

            // Transacao
            CreateMap<TransacaoRequest, Transacao>();
            CreateMap<Transacao, TransacaoResponse>()
                .ForMember(dest => dest.CategoriaNome, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nome : string.Empty));
        }
    }
}
