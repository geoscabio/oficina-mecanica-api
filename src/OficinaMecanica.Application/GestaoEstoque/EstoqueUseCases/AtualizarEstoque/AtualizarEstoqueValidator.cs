using FluentValidation;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;

public sealed class AtualizarEstoqueValidator : AbstractValidator<AtualizarEstoqueRequest>
{
    public AtualizarEstoqueValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(EstoqueValidationMessages.RequestAtualizarEstoqueObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.PecaInsumoCatalogoId)
                .NotEmpty()
                .WithMessage(EstoqueValidationMessages.PecaInsumoCatalogoObrigatorio);

            RuleFor(request => request.QuantidadeDisponivel)
                .GreaterThanOrEqualTo(0)
                .WithMessage(EstoqueValidationMessages.QuantidadeDisponivelNaoNegativa);
        });
    }
}