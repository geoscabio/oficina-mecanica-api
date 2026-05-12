using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Administrativo.Exceptions;

public sealed class ServicoCatalogoInvalidoException : DomainException
{
    public ServicoCatalogoInvalidoException(string message)
        : base(message)
    {
    }
}
