using FluentValidation;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

public sealed class RegistrarEntradaEstoqueValidator : AbstractValidator<RegistrarEntradaEstoqueRequest>
{
    public RegistrarEntradaEstoqueValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(EstoqueValidationMessages.RequestRegistrarEntradaEstoqueObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.PecaInsumoCatalogoId)
                .NotEmpty()
                .WithMessage(EstoqueValidationMessages.PecaInsumoCatalogoObrigatorio);

            RuleFor(request => request.Quantidade)
                .GreaterThan(0)
                .WithMessage(EstoqueValidationMessages.QuantidadeMaiorQueZero);
        });
    }
}