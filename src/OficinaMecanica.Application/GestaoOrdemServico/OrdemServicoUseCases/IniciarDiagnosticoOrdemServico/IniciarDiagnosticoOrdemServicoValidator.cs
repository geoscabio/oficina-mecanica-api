using FluentValidation;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed class IniciarDiagnosticoOrdemServicoValidator : AbstractValidator<IniciarDiagnosticoOrdemServicoRequest>
{
    public IniciarDiagnosticoOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request para iniciar diagnostico e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage("OrdemServicoId e obrigatorio.");
        });
    }
}
