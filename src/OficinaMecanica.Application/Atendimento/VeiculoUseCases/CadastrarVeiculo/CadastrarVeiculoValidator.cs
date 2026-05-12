using OficinaMecanica.Application.Common.Exceptions;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed class CadastrarVeiculoValidator
{
    public void Validate(CadastrarVeiculoRequest request)
    {
        if (request is null)
        {
            throw new ValidationException("Request de cadastro de veiculo e obrigatorio.");
        }

        if (request.ClienteId == Guid.Empty)
        {
            throw new ValidationException("ClienteId e obrigatorio.");
        }

        ValidarObrigatorio(request.Placa, "Placa");
        ValidarObrigatorio(request.Marca, "Marca");
        ValidarObrigatorio(request.Modelo, "Modelo");

        if (request.Ano <= 0)
        {
            throw new ValidationException("Ano deve ser maior que zero.");
        }
    }

    private static void ValidarObrigatorio(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ValidationException($"{campo} e obrigatorio.");
        }
    }
}
