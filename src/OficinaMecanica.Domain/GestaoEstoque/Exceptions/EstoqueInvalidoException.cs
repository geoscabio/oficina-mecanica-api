using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoEstoque.Exceptions;

public sealed class EstoqueInvalidoException : DomainException
{
    public EstoqueInvalidoException(string message)
        : base(message)
    {
    }
}
