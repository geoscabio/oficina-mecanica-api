using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteValidator : AbstractValidator<ConsultarClienteRequest>
{
    public ConsultarClienteValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request invalido.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage("Id do cliente e obrigatorio.");
        });
    }
}
