using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;

public sealed class ListarServicosCatalogoValidator : AbstractValidator<ListarServicosCatalogoRequest>
{
    public ListarServicosCatalogoValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(ServicoCatalogoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(ServicoCatalogoValidationMessages.TamanhoPaginaInvalido);
    }
}
