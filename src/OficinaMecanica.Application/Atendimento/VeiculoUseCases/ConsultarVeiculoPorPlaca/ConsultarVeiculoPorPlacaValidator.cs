using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public sealed class ConsultarVeiculoPorPlacaValidator : AbstractValidator<ConsultarVeiculoPorPlacaRequest>
{
    public ConsultarVeiculoPorPlacaValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ValidationErrorMessages.RequestInvalido);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Placa)
                .NotEmpty()
                .WithMessage(VeiculoValidationMessages.PlacaVeiculoObrigatoria);
        });
    }
}

