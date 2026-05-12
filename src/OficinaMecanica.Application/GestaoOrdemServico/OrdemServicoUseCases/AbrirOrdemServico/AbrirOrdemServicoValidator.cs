using FluentValidation;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed class AbrirOrdemServicoValidator : AbstractValidator<AbrirOrdemServicoRequest>
{
    public AbrirOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request de abertura de ordem de servico e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.VeiculoId)
                .NotEmpty()
                .WithMessage("VeiculoId e obrigatorio.");

            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage("MecanicoId e obrigatorio.");
        });
    }
}
