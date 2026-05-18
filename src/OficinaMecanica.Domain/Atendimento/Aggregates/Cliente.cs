using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.Atendimento.Aggregates;

public sealed class Cliente
{
    private Cliente()
    {
        Documento = null!;
        Nome = string.Empty;
        Endereco = null!;
        Telefone = null!;
        Email = null!;
    }

    private Cliente(Guid id, CpfCnpj documento, string nome, Endereco endereco, Telefone telefone, Email email)
    {
        Id = id;
        Documento = documento;
        Nome = nome;
        Endereco = endereco;
        Telefone = telefone;
        Email = email;
    }

    public Guid Id { get; private set; }
    public CpfCnpj Documento { get; private set; }
    public string Nome { get; private set; }
    public Endereco Endereco { get; private set; }
    public Telefone Telefone { get; private set; }
    public Email Email { get; private set; }

    public static Cliente Criar(CpfCnpj documento, string nome, Endereco endereco, Telefone telefone, Email email)
    {
        if (documento is null)
        {
            throw new DomainException(ClienteErrorMessages.DocumentoObrigatorio);
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException(ClienteErrorMessages.NomeObrigatorio);
        }

        return new Cliente(
            Guid.NewGuid(),
            documento,
            nome.Trim(),
            endereco ?? throw new DomainException(ClienteErrorMessages.EnderecoObrigatorio),
            telefone ?? throw new DomainException(ClienteErrorMessages.TelefoneObrigatorio),
            email ?? throw new DomainException(ClienteErrorMessages.EmailObrigatorio));
    }

    public void Atualizar(string nome, Endereco endereco, Telefone telefone, Email email)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException(ClienteErrorMessages.NomeObrigatorio);
        }

        Nome = nome.Trim();
        Endereco = endereco ?? throw new DomainException(ClienteErrorMessages.EnderecoObrigatorio);
        Telefone = telefone ?? throw new DomainException(ClienteErrorMessages.TelefoneObrigatorio);
        Email = email ?? throw new DomainException(ClienteErrorMessages.EmailObrigatorio);
    }
}

