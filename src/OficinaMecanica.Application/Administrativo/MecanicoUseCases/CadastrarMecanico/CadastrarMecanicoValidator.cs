using FluentValidation;
using OficinaMecanica.Application.Administrativo.ValidationMessages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;

public sealed class CadastrarMecanicoValidator : AbstractValidator<CadastrarMecanicoRequest>
{
    public CadastrarMecanicoValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(MecanicoValidationMessages.RequestCadastrarMecanicoObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Nome)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.NomeObrigatorio);

            RuleFor(request => request.Funcional)
                .NotEmpty()
                .WithMessage(MecanicoValidationMessages.FuncionalObrigatorio);
        });
    }
}