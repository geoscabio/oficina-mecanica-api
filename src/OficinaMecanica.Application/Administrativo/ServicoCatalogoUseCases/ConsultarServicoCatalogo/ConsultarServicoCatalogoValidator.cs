using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;

public sealed class ConsultarServicoCatalogoValidator : AbstractValidator<ConsultarServicoCatalogoRequest>
{
    public ConsultarServicoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ServicoCatalogoValidationMessages.RequestConsultarServicoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ServicoCatalogoId)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.ServicoCatalogoIdObrigatorio);
        });
    }
}
