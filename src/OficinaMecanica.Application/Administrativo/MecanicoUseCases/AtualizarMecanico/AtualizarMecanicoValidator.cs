using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;

public sealed class AtualizarMecanicoValidator : AbstractValidator<AtualizarMecanicoRequest>
{
    public AtualizarMecanicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(MecanicoValidationMessages.RequestAtualizarMecanicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.MecanicoId)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.IdMecanicoObrigatorio);

            RuleFor(request => request.Nome)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.NomeObrigatorio);

            RuleFor(request => request.Funcional)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.FuncionalObrigatorio);
        });
    }
}