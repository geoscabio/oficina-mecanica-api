using FluentValidation;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;

public sealed class ConsultarItemEstoqueValidator : AbstractValidator<ConsultarItemEstoqueRequest>
{
    public ConsultarItemEstoqueValidator()
    {
        RuleFor(request => request.ItemEstoqueId)
            .NotEmpty()
            .WithMessage(EstoqueValidationMessages.IdItemEstoqueObrigatorio);
    }
}