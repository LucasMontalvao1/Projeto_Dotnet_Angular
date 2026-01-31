using FluentValidation;
using FinanceReminderHub.Application.DTOs.Requests;

namespace FinanceReminderHub.Application.Validators
{
    public class TransacaoRequestValidator : AbstractValidator<TransacaoRequest>
    {
        public TransacaoRequestValidator()
        {
            RuleFor(x => x.Valor).GreaterThan(0);
            RuleFor(x => x.Descricao).MaximumLength(255);
            RuleFor(x => x.Data).GreaterThan(DateTime.MinValue);
            RuleFor(x => x.Tipo).Must(t => t == "Entrada" || t == "Saida").WithMessage("Tipo deve ser 'Entrada' ou 'Saida'");
        }
    }
}
