using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;

public sealed class ListarClientesValidator : AbstractValidator<ListarClientesRequest>
{
    public ListarClientesValidator()
    {
        RuleFor(request => request.Pagina)
            .GreaterThan(0)
            .WithMessage(ClienteValidationMessages.PaginaMaiorQueZero);

        RuleFor(request => request.TamanhoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage(ClienteValidationMessages.TamanhoPaginaInvalido);
    }
}