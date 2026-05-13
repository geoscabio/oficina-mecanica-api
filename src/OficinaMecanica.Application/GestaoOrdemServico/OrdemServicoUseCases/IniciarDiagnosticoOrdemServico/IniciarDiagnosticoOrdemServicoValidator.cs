using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed class IniciarDiagnosticoOrdemServicoValidator : AbstractValidator<IniciarDiagnosticoOrdemServicoRequest>
{
    public IniciarDiagnosticoOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestIniciarDiagnosticoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);
        });
    }
}

