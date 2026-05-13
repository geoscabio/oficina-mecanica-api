using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteValidator : AbstractValidator<ConsultarClienteRequest>
{
    public ConsultarClienteValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ValidationErrorMessages.RequestInvalido);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.IdClienteObrigatorio);
        });
    }
}

