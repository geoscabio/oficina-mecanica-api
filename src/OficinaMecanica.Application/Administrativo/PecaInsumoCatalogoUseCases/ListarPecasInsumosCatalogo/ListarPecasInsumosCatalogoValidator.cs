using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;

public sealed class ListarPecasInsumosCatalogoValidator : AbstractValidator<ListarPecasInsumosCatalogoRequest>
{
    public ListarPecasInsumosCatalogoValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(PecaInsumoCatalogoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(PecaInsumoCatalogoValidationMessages.TamanhoPaginaInvalido);
    }
}