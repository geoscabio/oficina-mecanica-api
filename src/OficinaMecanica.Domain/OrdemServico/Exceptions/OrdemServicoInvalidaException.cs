using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.OrdemServico.Exceptions;

public sealed class OrdemServicoInvalidaException : DomainException
{
    public OrdemServicoInvalidaException(string message)
        : base(message)
    {
    }
}
