using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Atendimento.Exceptions;

public sealed class PlacaInvalidaException : DomainException
{
    public PlacaInvalidaException(string message)
        : base(message)
    {
    }
}
