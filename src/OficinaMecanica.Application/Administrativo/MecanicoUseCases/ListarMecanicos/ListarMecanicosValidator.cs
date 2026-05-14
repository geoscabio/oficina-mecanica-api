using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;

public sealed class ListarMecanicosValidator : AbstractValidator<ListarMecanicosRequest>
{
    public ListarMecanicosValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(MecanicoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(MecanicoValidationMessages.TamanhoPaginaInvalido);
    }
}