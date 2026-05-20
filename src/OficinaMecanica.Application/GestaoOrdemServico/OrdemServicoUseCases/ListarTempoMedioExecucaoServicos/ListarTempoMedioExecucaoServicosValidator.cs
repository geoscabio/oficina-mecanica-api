using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarTempoMedioExecucaoServicos;

public sealed class ListarTempoMedioExecucaoServicosValidator : AbstractValidator<ListarTempoMedioExecucaoServicosRequest>
{
    public ListarTempoMedioExecucaoServicosValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(OrdemServicoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(OrdemServicoValidationMessages.TamanhoPaginaInvalido);
    }
}
