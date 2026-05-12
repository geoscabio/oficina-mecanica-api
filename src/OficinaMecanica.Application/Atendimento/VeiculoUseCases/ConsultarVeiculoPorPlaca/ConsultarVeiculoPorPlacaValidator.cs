using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public sealed class ConsultarVeiculoPorPlacaValidator : AbstractValidator<ConsultarVeiculoPorPlacaRequest>
{
    public ConsultarVeiculoPorPlacaValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request invalido.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Placa)
                .NotEmpty()
                .WithMessage("Placa do veiculo e obrigatoria.");
        });
    }
}
