using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public sealed class ConsultarClientePorDocumentoValidator : AbstractValidator<ConsultarClientePorDocumentoRequest>
{
    public ConsultarClientePorDocumentoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ValidationErrorMessages.RequestInvalido);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Documento)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.DocumentoClienteObrigatorio);
        });
    }
}

