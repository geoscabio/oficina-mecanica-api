using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Atendimento.Exceptions;

public sealed class ClienteInvalidoException : DomainException
{
    public ClienteInvalidoException(string message)
        : base(message)
    {
    }
}
