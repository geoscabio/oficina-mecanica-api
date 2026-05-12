using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Administrativo.Exceptions;

public sealed class PecaInsumoCatalogoInvalidaException : DomainException
{
    public PecaInsumoCatalogoInvalidaException(string message)
        : base(message)
    {
    }
}
