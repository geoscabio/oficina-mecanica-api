using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarTempoMedioExecucaoServico;

public sealed class ConsultarTempoMedioExecucaoServicoValidator : AbstractValidator<ConsultarTempoMedioExecucaoServicoRequest>
{
    public ConsultarTempoMedioExecucaoServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestConsultarTempoMedioExecucaoServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ServicoCatalogoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.ServicoCatalogoIdObrigatorio);
        });
    }
}
