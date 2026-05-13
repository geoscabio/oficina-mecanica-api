using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;

public sealed class CancelarOrdemServicoValidator : AbstractValidator<CancelarOrdemServicoRequest>
{
    public CancelarOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestCancelarOrdemServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);

            RuleFor(request => request.Motivo)
                .Must(motivo => Enum.IsDefined(typeof(MotivoCancelamentoOrdemServico), motivo))
                .WithMessage(OrdemServicoValidationMessages.MotivoCancelamentoObrigatorio);
        });
    }
}
