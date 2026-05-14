using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;

public sealed class AtualizarVeiculoValidator : AbstractValidator<AtualizarVeiculoRequest>
{
    public AtualizarVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(VeiculoValidationMessages.RequestAtualizarVeiculoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.VeiculoId).NotEmpty().WithMessage(VeiculoValidationMessages.IdVeiculoObrigatorio);
            RuleFor(request => request.Placa).NotEmpty().WithMessage(VeiculoValidationMessages.PlacaObrigatoria);
            RuleFor(request => request.Marca).NotEmpty().WithMessage(VeiculoValidationMessages.MarcaObrigatoria);
            RuleFor(request => request.Modelo).NotEmpty().WithMessage(VeiculoValidationMessages.ModeloObrigatorio);
            RuleFor(request => request.Ano).GreaterThan(0).WithMessage(VeiculoValidationMessages.AnoMaiorQueZero);
        });
    }
}