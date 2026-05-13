using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed class AbrirOrdemServicoValidator : AbstractValidator<AbrirOrdemServicoRequest>
{
    public AbrirOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestAberturaOrdemServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.VeiculoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.VeiculoIdObrigatorio);

            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.MecanicoIdObrigatorio);
        });
    }
}

