using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;

public sealed class RemoverServicoCatalogoValidator : AbstractValidator<RemoverServicoCatalogoRequest>
{
    public RemoverServicoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ServicoCatalogoValidationMessages.RequestRemoverServicoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ServicoCatalogoId)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.ServicoCatalogoIdObrigatorio);
        });
    }
}
