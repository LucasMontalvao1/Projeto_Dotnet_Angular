using FluentValidation;
using FinanceReminderHub.Application.DTOs.Requests;

namespace FinanceReminderHub.Application.Validators
{
    public class CategoriaRequestValidator : AbstractValidator<CategoriaRequest>
    {
        public CategoriaRequestValidator()
        {
            RuleFor(x => x.Nome).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descricao).MaximumLength(255);
        }
    }
}
