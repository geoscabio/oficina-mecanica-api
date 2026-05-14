using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;

public sealed class ConsultarMecanicoValidator : AbstractValidator<ConsultarMecanicoRequest>
{
    public ConsultarMecanicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(MecanicoValidationMessages.RequestConsultarMecanicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.IdMecanicoObrigatorio);
        });
    }
}