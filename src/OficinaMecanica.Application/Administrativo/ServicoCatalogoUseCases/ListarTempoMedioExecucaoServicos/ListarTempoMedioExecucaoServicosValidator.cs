using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;

public sealed class ListarTempoMedioExecucaoServicosValidator : AbstractValidator<ListarTempoMedioExecucaoServicosRequest>
{
    public ListarTempoMedioExecucaoServicosValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(ServicoCatalogoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(ServicoCatalogoValidationMessages.TamanhoPaginaInvalido);
    }
}
