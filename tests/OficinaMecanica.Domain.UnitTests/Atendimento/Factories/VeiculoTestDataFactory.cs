using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

internal static class VeiculoTestDataFactory
{
    public const string MarcaPadrao = "Toyota";
    public const string ModeloPadrao = "Corolla";
    public const int AnoPadrao = 2020;

    public static Placa CriarPlacaPadrao()
    {
        return Placa.Criar("ABC-1234");
    }

    public static Veiculo CriarVeiculoPadrao(Guid? clienteId = null)
    {
        return Veiculo.Criar(
            clienteId ?? Guid.NewGuid(),
            CriarPlacaPadrao(),
            MarcaPadrao,
            ModeloPadrao,
            AnoPadrao);
    }
}
