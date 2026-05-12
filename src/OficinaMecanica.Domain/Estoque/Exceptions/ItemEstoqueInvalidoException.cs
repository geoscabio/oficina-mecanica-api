using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Estoque.Exceptions;

public sealed class ItemEstoqueInvalidoException : DomainException
{
    public ItemEstoqueInvalidoException(string message)
        : base(message)
    {
    }
}
