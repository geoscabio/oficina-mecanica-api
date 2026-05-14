using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.RemoverVeiculo;

public sealed class RemoverVeiculoValidator : AbstractValidator<RemoverVeiculoRequest>
{
    public RemoverVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(VeiculoValidationMessages.RequestRemoverVeiculoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.VeiculoId)
                .NotEmpty()
                .WithMessage(VeiculoValidationMessages.IdVeiculoObrigatorio);
        });
    }
}