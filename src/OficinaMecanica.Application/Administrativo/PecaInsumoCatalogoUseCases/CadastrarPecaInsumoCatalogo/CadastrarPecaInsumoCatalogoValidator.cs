using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;

public sealed class CadastrarPecaInsumoCatalogoValidator : AbstractValidator<CadastrarPecaInsumoCatalogoRequest>
{
    public CadastrarPecaInsumoCatalogoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(PecaInsumoCatalogoValidationMessages.RequestCadastrarPecaInsumoCatalogoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Descricao)
                .NotEmpty()
                .WithMessage(PecaInsumoCatalogoValidationMessages.DescricaoObrigatoria);

            RuleFor(request => request.Tipo)
                .Must(tipo => Enum.IsDefined(tipo))
                .WithMessage(PecaInsumoCatalogoValidationMessages.TipoInvalido);

            RuleFor(request => request.Valor)
                .GreaterThan(0)
                .WithMessage(PecaInsumoCatalogoValidationMessages.ValorMaiorQueZero);
        });
    }
}
