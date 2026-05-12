using FluentValidation;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed class CadastrarClienteValidator : AbstractValidator<CadastrarClienteRequest>
{
    public CadastrarClienteValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Request de cadastro de cliente e obrigatorio.");

        When(request => request is not null, () =>
        {
            RuleFor(request => request.Documento).NotEmpty().WithMessage("Documento e obrigatorio.");
            RuleFor(request => request.Nome).NotEmpty().WithMessage("Nome e obrigatorio.");
            RuleFor(request => request.Logradouro).NotEmpty().WithMessage("Logradouro e obrigatorio.");
            RuleFor(request => request.Numero).NotEmpty().WithMessage("Numero e obrigatorio.");
            RuleFor(request => request.Bairro).NotEmpty().WithMessage("Bairro e obrigatorio.");
            RuleFor(request => request.Cidade).NotEmpty().WithMessage("Cidade e obrigatorio.");
            RuleFor(request => request.CEP).NotEmpty().WithMessage("CEP e obrigatorio.");
            RuleFor(request => request.Telefone).NotEmpty().WithMessage("Telefone e obrigatorio.");
            RuleFor(request => request.Email).NotEmpty().WithMessage("Email e obrigatorio.");
        });
    }
}
