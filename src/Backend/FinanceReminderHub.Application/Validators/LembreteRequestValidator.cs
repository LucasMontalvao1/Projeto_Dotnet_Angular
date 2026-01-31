using FluentValidation;
using FinanceReminderHub.Application.DTOs.Requests;

namespace FinanceReminderHub.Application.Validators
{
    public class LembreteRequestValidator : AbstractValidator<LembreteRequest>
    {
        public LembreteRequestValidator()
        {
            RuleFor(x => x.Titulo).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Descricao).NotEmpty().MaximumLength(500);
            RuleFor(x => x.DataLembrete).GreaterThan(DateTime.MinValue);
            RuleFor(x => x.IntervaloEmDias).GreaterThan(0);
        }
    }
}
