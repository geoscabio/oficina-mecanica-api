using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

public sealed class ServicoInvalidoException : DomainException
{
    public ServicoInvalidoException(string message)
        : base(message)
    {
    }
}
