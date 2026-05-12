using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public sealed class ConsultarClientePorDocumentoValidator : AbstractValidator<ConsultarClientePorDocumentoRequest>
{
    public ConsultarClientePorDocumentoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request invalido.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Documento)
                .NotEmpty()
                .WithMessage("Documento do cliente e obrigatorio.");
        });
    }
}
