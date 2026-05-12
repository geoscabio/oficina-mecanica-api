using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Administrativo.Exceptions;

public sealed class MecanicoInvalidoException : DomainException
{
    public MecanicoInvalidoException(string message)
        : base(message)
    {
    }
}
