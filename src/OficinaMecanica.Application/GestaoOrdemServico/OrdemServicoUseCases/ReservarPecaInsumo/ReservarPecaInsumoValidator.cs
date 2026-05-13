using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

public sealed class ReservarPecaInsumoValidator : AbstractValidator<ReservarPecaInsumoRequest>
{
    public ReservarPecaInsumoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestReservarPecaInsumoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);

            RuleFor(request => request.PecasInsumos)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.PecasInsumosObrigatorio);

            RuleFor(request => request.PecasInsumos)
                .Must(pecasInsumos => pecasInsumos.Select(item => item.PecaInsumoCatalogoId).Distinct().Count() == pecasInsumos.Count)
                .WithMessage(OrdemServicoValidationMessages.PecasInsumosSemItensRepetidos);

            RuleForEach(request => request.PecasInsumos)
                .ChildRules(pecaInsumo =>
                {
                    pecaInsumo.RuleFor(item => item.PecaInsumoCatalogoId)
                        .NotEmpty()
                        .WithMessage(OrdemServicoValidationMessages.PecaInsumoCatalogoIdObrigatorio);

                    pecaInsumo.RuleFor(item => item.Quantidade)
                        .GreaterThan(0)
                        .WithMessage(OrdemServicoValidationMessages.QuantidadeMaiorQueZero);
                });
        });
    }
}

