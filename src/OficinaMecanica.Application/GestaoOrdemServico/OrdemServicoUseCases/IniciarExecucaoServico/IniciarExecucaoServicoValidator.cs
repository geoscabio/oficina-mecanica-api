using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;

public sealed class IniciarExecucaoServicoValidator : AbstractValidator<IniciarExecucaoServicoRequest>
{
    public IniciarExecucaoServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestIniciarExecucaoServicoObrigatorio);

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
