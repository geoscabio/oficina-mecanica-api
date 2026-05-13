using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;

public sealed class ConsultarTempoMedioExecucaoServicoValidator : AbstractValidator<ConsultarTempoMedioExecucaoServicoRequest>
{
    public ConsultarTempoMedioExecucaoServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ServicoCatalogoValidationMessages.RequestConsultarTempoMedioExecucaoServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ServicoCatalogoId)
                .NotEmpty()
                .WithMessage(ServicoCatalogoValidationMessages.ServicoCatalogoIdObrigatorio);
        });
    }
}
