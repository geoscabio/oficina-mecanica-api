using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Domain.Atendimento.Exceptions;

public sealed class VeiculoInvalidoException : DomainException
{
    public VeiculoInvalidoException(string message)
        : base(message)
    {
    }
}
