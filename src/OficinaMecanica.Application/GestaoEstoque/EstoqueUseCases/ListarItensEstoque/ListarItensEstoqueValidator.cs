using FluentValidation;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;

public sealed class ListarItensEstoqueValidator : AbstractValidator<ListarItensEstoqueRequest>
{
    public ListarItensEstoqueValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(EstoqueValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(EstoqueValidationMessages.TamanhoPaginaInvalido);
    }
}