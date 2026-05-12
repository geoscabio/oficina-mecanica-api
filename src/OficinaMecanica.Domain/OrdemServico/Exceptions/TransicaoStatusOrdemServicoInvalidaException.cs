using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.OrdemServico.Exceptions;

public sealed class TransicaoStatusOrdemServicoInvalidaException : DomainException
{
    public TransicaoStatusOrdemServicoInvalidaException(string message)
        : base(message)
    {
    }
}
