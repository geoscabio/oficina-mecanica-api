using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Domain.Administrativo.Aggregates;

public sealed class Mecanico
{
    private Mecanico(Guid id, string nome, string funcional)
    {
        Id = id;
        Nome = nome;
        Funcional = funcional;
    }

    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Funcional { get; private set; }

    public static Mecanico Criar(string nome, string funcional)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException(MecanicoErrorMessages.NomeObrigatorio);
        }

        if (string.IsNullOrWhiteSpace(funcional))
        {
            throw new DomainException(MecanicoErrorMessages.FuncionalObrigatorio);
        }

        return new Mecanico(Guid.NewGuid(), nome.Trim(), funcional.Trim());
    }
}

