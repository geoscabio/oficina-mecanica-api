using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Atendimento.Exceptions;

public sealed class DocumentoInvalidoException : DomainException
{
    public DocumentoInvalidoException(string message)
        : base(message)
    {
    }
}
