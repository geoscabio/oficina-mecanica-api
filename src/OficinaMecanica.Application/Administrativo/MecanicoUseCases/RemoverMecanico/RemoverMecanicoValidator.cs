using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;

public sealed class RemoverMecanicoValidator : AbstractValidator<RemoverMecanicoRequest>
{
    public RemoverMecanicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(MecanicoValidationMessages.RequestRemoverMecanicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.IdMecanicoObrigatorio);
        });
    }
}