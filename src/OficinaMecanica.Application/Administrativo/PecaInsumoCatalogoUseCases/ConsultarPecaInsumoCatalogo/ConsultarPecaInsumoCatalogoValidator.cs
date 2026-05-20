using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;

public sealed class ConsultarPecaInsumoCatalogoValidator : AbstractValidator<ConsultarPecaInsumoCatalogoRequest>
{
    public ConsultarPecaInsumoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(PecaInsumoCatalogoValidationMessages.RequestConsultarPecaInsumoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.PecaInsumoCatalogoId)
                .NotEmpty()
                .WithMessage(PecaInsumoCatalogoValidationMessages.IdPecaInsumoCatalogoObrigatorio);
        });
    }
}