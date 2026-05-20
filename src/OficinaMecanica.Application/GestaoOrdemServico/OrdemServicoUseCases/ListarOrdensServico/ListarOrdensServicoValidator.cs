using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;

public sealed class ListarOrdensServicoValidator : AbstractValidator<ListarOrdensServicoRequest>
{
    public ListarOrdensServicoValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(OrdemServicoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(OrdemServicoValidationMessages.TamanhoPaginaInvalido);
    }
}
