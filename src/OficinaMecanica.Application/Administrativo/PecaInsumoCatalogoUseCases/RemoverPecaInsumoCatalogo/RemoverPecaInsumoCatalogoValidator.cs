using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

public sealed class RemoverPecaInsumoCatalogoValidator : AbstractValidator<RemoverPecaInsumoCatalogoRequest>
{
    public RemoverPecaInsumoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(PecaInsumoCatalogoValidationMessages.RequestRemoverPecaInsumoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.PecaInsumoCatalogoId)
                .NotEmpty()
                .WithMessage(PecaInsumoCatalogoValidationMessages.IdPecaInsumoCatalogoObrigatorio);
        });
    }
}