using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.NotificarDecisaoOrcamento;

public sealed class NotificarDecisaoOrcamentoValidator : AbstractValidator<NotificarDecisaoOrcamentoRequest>
{
    public NotificarDecisaoOrcamentoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestNotificarDecisaoOrcamentoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);

            RuleFor(request => request.Decisao)
                .Must(decisao => Enum.IsDefined(decisao))
                .WithMessage(OrdemServicoValidationMessages.DecisaoOrcamentoObrigatoria);
        });
    }
}
