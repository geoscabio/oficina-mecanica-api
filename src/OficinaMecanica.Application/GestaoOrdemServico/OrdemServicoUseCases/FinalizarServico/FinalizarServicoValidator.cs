using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarServico;

public sealed class FinalizarServicoValidator : AbstractValidator<FinalizarServicoRequest>
{
    public FinalizarServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestFinalizarServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);

            RuleFor(request => request.ServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.ServicoIdObrigatorio);
        });
    }
}
