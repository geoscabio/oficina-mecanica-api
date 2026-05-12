using OficinaMecanica.Domain.Administrativo.Exceptions;

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
            throw new MecanicoInvalidoException("Nome do mecanico e obrigatorio.");
        }

        if (string.IsNullOrWhiteSpace(funcional))
        {
            throw new MecanicoInvalidoException("Funcional do mecanico e obrigatorio.");
        }

        return new Mecanico(Guid.NewGuid(), nome.Trim(), funcional.Trim());
    }
}
