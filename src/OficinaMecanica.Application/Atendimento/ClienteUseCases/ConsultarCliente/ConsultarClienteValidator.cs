using OficinaMecanica.Application.Common.Exceptions;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteValidator
{
    public void Validate(ConsultarClienteRequest request)
    {
        if (request is null)
        {
            throw new ValidationException("Request invalido.");
        }

        if (request.Id == Guid.Empty)
        {
            throw new ValidationException("Id do cliente e obrigatorio.");
        }
    }
}
