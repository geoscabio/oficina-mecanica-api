using FluentValidation;
using OficinaMecanica.Application.Atendimento.ValidationMessages;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;

public sealed class AtualizarClienteValidator : AbstractValidator<AtualizarClienteRequest>
{
    public AtualizarClienteValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage(ClienteValidationMessages.RequestAtualizarClienteObrigatorio);

        When(request => request is not null, () =>
        {
            RuleFor(request => request.ClienteId)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.IdClienteObrigatorio);

            RuleFor(request => request.Nome)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.NomeObrigatorio);

            RuleFor(request => request.Endereco)
                .NotNull()
                .WithMessage(ClienteValidationMessages.EnderecoObrigatorio);

            RuleFor(request => request.Telefone)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.TelefoneObrigatorio);

            RuleFor(request => request.Email)
                .NotEmpty()
                .WithMessage(ClienteValidationMessages.EmailObrigatorio);

            When(request => request.Endereco is not null, () =>
            {
                RuleFor(request => request.Endereco.Logradouro).NotEmpty().WithMessage(ClienteValidationMessages.LogradouroObrigatorio);
                RuleFor(request => request.Endereco.Numero).NotEmpty().WithMessage(ClienteValidationMessages.NumeroObrigatorio);
                RuleFor(request => request.Endereco.Bairro).NotEmpty().WithMessage(ClienteValidationMessages.BairroObrigatorio);
                RuleFor(request => request.Endereco.Cidade).NotEmpty().WithMessage(ClienteValidationMessages.CidadeObrigatoria);
                RuleFor(request => request.Endereco.CEP).NotEmpty().WithMessage(ClienteValidationMessages.CepObrigatorio);
            });
        });
    }
}