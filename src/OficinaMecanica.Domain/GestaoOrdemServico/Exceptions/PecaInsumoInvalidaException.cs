using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

public sealed class PecaInsumoInvalidaException : DomainException
{
    public PecaInsumoInvalidaException(string message)
        : base(message)
    {
    }
}
