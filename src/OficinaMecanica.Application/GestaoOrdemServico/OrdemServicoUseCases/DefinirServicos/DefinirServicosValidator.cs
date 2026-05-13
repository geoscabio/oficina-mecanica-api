using FluentValidation;
using OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;

public sealed class DefinirServicosValidator : AbstractValidator<DefinirServicosRequest>
{
    public DefinirServicosValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(OrdemServicoValidationMessages.RequestDefinirServicosObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.OrdemServicoId)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.OrdemServicoIdObrigatorio);

            RuleFor(request => request.ServicosCatalogoIds)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.ServicosCatalogoIdsObrigatorio);

            RuleForEach(request => request.ServicosCatalogoIds)
                .NotEmpty()
                .WithMessage(OrdemServicoValidationMessages.ServicoCatalogoIdObrigatorio);
        });
    }
}

