using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoValidator : AbstractValidator<ConsultarVeiculoRequest>
{
    public ConsultarVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request invalido.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage("Id do veiculo e obrigatorio.");
        });
    }
}
