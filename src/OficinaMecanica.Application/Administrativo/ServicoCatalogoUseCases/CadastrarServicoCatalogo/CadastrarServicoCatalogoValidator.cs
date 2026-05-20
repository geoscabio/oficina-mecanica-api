using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;

public sealed class CadastrarServicoCatalogoValidator : AbstractValidator<CadastrarServicoCatalogoRequest>
{
    public CadastrarServicoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ServicoCatalogoValidationMessages.RequestCadastrarServicoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Descricao)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.DescricaoObrigatoria);

            RuleFor(request => request.Valor)
                .GreaterThan(0)
                .WithMessage(ServicoCatalogoValidationMessages.ValorMaiorQueZero);
        });
    }
}
