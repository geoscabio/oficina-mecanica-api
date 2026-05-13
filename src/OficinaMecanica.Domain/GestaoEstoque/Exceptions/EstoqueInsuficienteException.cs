using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoEstoque.Exceptions;

public sealed class EstoqueInsuficienteException : DomainException
{
    public EstoqueInsuficienteException(string message)
        : base(message)
    {
    }
}
