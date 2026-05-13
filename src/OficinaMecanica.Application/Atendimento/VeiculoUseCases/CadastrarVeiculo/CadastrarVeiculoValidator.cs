using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed class CadastrarVeiculoValidator : AbstractValidator<CadastrarVeiculoRequest>
{
    public CadastrarVeiculoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(VeiculoValidationMessages.RequestCadastroVeiculoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ClienteId).NotEmpty().WithMessage(VeiculoValidationMessages.ClienteIdObrigatorio);
            RuleFor(request => request.Placa).NotEmpty().WithMessage(VeiculoValidationMessages.PlacaObrigatoria);
            RuleFor(request => request.Marca).NotEmpty().WithMessage(VeiculoValidationMessages.MarcaObrigatoria);
            RuleFor(request => request.Modelo).NotEmpty().WithMessage(VeiculoValidationMessages.ModeloObrigatorio);
            RuleFor(request => request.Ano).GreaterThan(0).WithMessage(VeiculoValidationMessages.AnoMaiorQueZero);
        });
    }
}

