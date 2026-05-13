using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoValidator : AbstractValidator<ConsultarVeiculoRequest>
{
    public ConsultarVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ValidationErrorMessages.RequestInvalido);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Id)
                .NotEmpty()
                .WithMessage(VeiculoValidationMessages.IdVeiculoObrigatorio);
        });
    }
}

