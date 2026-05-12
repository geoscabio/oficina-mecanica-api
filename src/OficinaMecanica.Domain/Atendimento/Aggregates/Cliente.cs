using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.Atendimento.Aggregates;

public sealed class Cliente
{
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
            throw new ClienteInvalidoException("Documento do cliente e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ClienteInvalidoException("Nome do cliente e obrigatorio.");
        }

        return new Cliente(
            Guid.NewGuid(),
            documento,
            nome.Trim(),
            endereco ?? throw new ClienteInvalidoException("Endereco do cliente e obrigatorio."),
            telefone ?? throw new ClienteInvalidoException("Telefone do cliente e obrigatorio."),
            email ?? throw new ClienteInvalidoException("E-mail do cliente e obrigatorio."));
    }
}
