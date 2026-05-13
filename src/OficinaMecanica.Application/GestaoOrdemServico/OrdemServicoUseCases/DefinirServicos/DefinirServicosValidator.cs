using FluentValidation;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;

public sealed class DefinirServicosValidator : AbstractValidator<DefinirServicosRequest>
{
    public DefinirServicosValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request para definir servicos e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage("OrdemServicoId e obrigatorio.");

            RuleFor(request => request.ServicosCatalogoIds)
                .NotEmpty()
                .WithMessage("ServicosCatalogoIds e obrigatorio.");

            RuleForEach(request => request.ServicosCatalogoIds)
                .NotEmpty()
                .WithMessage("ServicoCatalogoId e obrigatorio.");
        });
    }
}
