using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

public sealed class TransicaoStatusOrdemServicoInvalidaException : DomainException
{
    public TransicaoStatusOrdemServicoInvalidaException(string message)
        : base(message)
    {
    }
}
