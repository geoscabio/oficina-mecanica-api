using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed class CadastrarVeiculoValidator : AbstractValidator<CadastrarVeiculoRequest>
{
    public CadastrarVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request de cadastro de veiculo e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ClienteId).NotEmpty().WithMessage("ClienteId e obrigatorio.");
            RuleFor(request => request.Placa).NotEmpty().WithMessage("Placa e obrigatorio.");
            RuleFor(request => request.Marca).NotEmpty().WithMessage("Marca e obrigatorio.");
            RuleFor(request => request.Modelo).NotEmpty().WithMessage("Modelo e obrigatorio.");
            RuleFor(request => request.Ano).GreaterThan(0).WithMessage("Ano deve ser maior que zero.");
        });
    }
}
