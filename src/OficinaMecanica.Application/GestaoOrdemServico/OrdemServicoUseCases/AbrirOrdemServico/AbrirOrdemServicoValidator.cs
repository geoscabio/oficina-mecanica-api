using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed class AbrirOrdemServicoValidator : AbstractValidator<AbrirOrdemServicoRequest>
{
    public AbrirOrdemServicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestAberturaOrdemServicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request)
                .Must(request => (request.ClienteId.HasValue && request.ClienteId.Value != Guid.Empty)
                    || !string.IsNullOrWhiteSpace(request.DocumentoCliente))
                .WithMessage(OrdemServicoValidationMessages.ClienteIdOuDocumentoObrigatorio);

            RuleFor(request => request.VeiculoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.VeiculoIdObrigatorio);

            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.MecanicoIdObrigatorio);

            RuleFor(request => request.ServicosCatalogoIds)
                .NotNull()
                .WithMessage(OrdemServicoValidationMessages.ServicosCatalogoIdsObrigatorio);

            RuleForEach(request => request.ServicosCatalogoIds)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.ServicoCatalogoIdObrigatorio)
                .When(request => request.ServicosCatalogoIds is not null);

            RuleFor(request => request.PecasInsumos)
                .NotNull()
                .WithMessage(OrdemServicoValidationMessages.PecasInsumosObrigatorio);

            RuleFor(request => request.PecasInsumos!)
                .Must(pecasInsumos => pecasInsumos
                    .Select(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId)
                    .Distinct()
                    .Count() == pecasInsumos.Count)
                .WithMessage(OrdemServicoValidationMessages.PecasInsumosSemItensRepetidos)
                .When(request => request.PecasInsumos is not null && request.PecasInsumos.Count > 0);

            RuleForEach(request => request.PecasInsumos)
                .ChildRules(pecaInsumo =>
                {
                    pecaInsumo.RuleFor(item => item.PecaInsumoCatalogoId)
                        .NotEmpty()
                        .WithMessage(OrdemServicoValidationMessages.PecaInsumoCatalogoIdObrigatorio);

                    pecaInsumo.RuleFor(item => item.Quantidade)
                        .GreaterThan(0)
                        .WithMessage(OrdemServicoValidationMessages.QuantidadeMaiorQueZero);
                })
                .When(request => request.PecasInsumos is not null);
        });
    }
}

