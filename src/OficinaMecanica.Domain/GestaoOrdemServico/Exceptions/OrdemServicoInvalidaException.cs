using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

public sealed class OrdemServicoInvalidaException : DomainException
{
    public OrdemServicoInvalidaException(string message)
        : base(message)
    {
    }
}
