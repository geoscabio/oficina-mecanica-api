using OficinaMecanica.Application.Common.Exceptions;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoValidator
{
    public void Validate(ConsultarVeiculoRequest request)
    {
        if (request is null)
        {
            throw new ValidationException("Request invalido.");
        }

        if (request.Id == Guid.Empty)
        {
            throw new ValidationException("Id do veiculo e obrigatorio.");
        }
    }
}
