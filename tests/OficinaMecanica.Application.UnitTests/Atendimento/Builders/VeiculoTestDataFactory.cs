using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.Builders;

internal static class VeiculoTestDataFactory
{
    public static Veiculo CriarVeiculoPadrao(Guid? clienteId = null)
    {
        return Veiculo.Criar(
            clienteId ?? Guid.NewGuid(),
            Placa.Criar("ABC-1234"),
            "Toyota",
            "Corolla",
            2020);
    }
}

