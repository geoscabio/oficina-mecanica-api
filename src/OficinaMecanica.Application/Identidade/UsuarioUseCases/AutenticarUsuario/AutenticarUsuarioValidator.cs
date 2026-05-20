using FluentValidation;
using OficinaMecanica.Application.Identidade.ValidationMessages;

namespace OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

public sealed class AutenticarUsuarioValidator : AbstractValidator<AutenticarUsuarioRequest>
{
    public AutenticarUsuarioValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(IdentidadeValidationMessages.RequestAutenticarUsuarioObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Login)
                .NotEmpty()
                .WithMessage(IdentidadeValidationMessages.LoginObrigatorio);

            RuleFor(request => request.Senha)
                .NotEmpty()
                .WithMessage(IdentidadeValidationMessages.SenhaObrigatoria);
        });
    }
}