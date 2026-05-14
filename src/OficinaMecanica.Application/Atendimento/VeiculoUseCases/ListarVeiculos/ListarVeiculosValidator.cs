using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;

public sealed class ListarVeiculosValidator : AbstractValidator<ListarVeiculosRequest>
{
    public ListarVeiculosValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(VeiculoValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(VeiculoValidationMessages.TamanhoPaginaInvalido);
    }
}