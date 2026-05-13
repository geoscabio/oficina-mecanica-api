using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;

public sealed class IniciarExecucaoOrdemServicoValidator : AbstractValidator<IniciarExecucaoOrdemServicoRequest>
{
    public IniciarExecucaoOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestIniciarExecucaoOrdemServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);
        });
    }
}
