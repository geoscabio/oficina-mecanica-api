using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;

public sealed class AtualizarServicoCatalogoValidator : AbstractValidator<AtualizarServicoCatalogoRequest>
{
    public AtualizarServicoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ServicoCatalogoValidationMessages.RequestAtualizarServicoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ServicoCatalogoId)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.ServicoCatalogoIdObrigatorio);

            RuleFor(request => request.Descricao)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.DescricaoObrigatoria);

            RuleFor(request => request.Valor)
                .GreaterThan(0)
                .WithMessage(ServicoCatalogoValidationMessages.ValorMaiorQueZero);
        });
    }
}
