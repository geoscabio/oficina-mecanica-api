using FluentValidation;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

public sealed class ReservarPecaInsumoValidator : AbstractValidator<ReservarPecaInsumoRequest>
{
    public ReservarPecaInsumoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request para reservar peca ou insumo e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage("OrdemServicoId e obrigatorio.");

            RuleFor(request => request.PecasInsumos)
                .NotEmpty()
                .WithMessage("PecasInsumos e obrigatorio.");

            RuleFor(request => request.PecasInsumos)
                .Must(pecasInsumos => pecasInsumos.Select(item => item.PecaInsumoCatalogoId).Distinct().Count() == pecasInsumos.Count)
                .WithMessage("PecasInsumos nao pode possuir itens repetidos.");

            RuleForEach(request => request.PecasInsumos)
                .ChildRules(pecaInsumo =>
                {
                    pecaInsumo.RuleFor(item => item.PecaInsumoCatalogoId)
                        .NotEmpty()
                        .WithMessage("PecaInsumoCatalogoId e obrigatorio.");

                    pecaInsumo.RuleFor(item => item.Quantidade)
                        .GreaterThan(0)
                        .WithMessage("Quantidade deve ser maior que zero.");
                });
        });
    }
}
