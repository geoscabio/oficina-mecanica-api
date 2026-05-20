using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;

public sealed class RemoverClienteValidator : AbstractValidator<RemoverClienteRequest>
{
    public RemoverClienteValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ClienteValidationMessages.RequestRemoverClienteObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ClienteId)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.IdClienteObrigatorio);
        });
    }
}