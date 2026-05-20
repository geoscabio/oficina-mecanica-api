using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

internal static class VeiculoTestDataFactory
{
    public const string PlacaPadrao = "ABC-1234";
    public const string PlacaNormalizadaPadrao = "ABC1234";

    public const string PlacaAtualizada = "XYZ-9876";
    public const string PlacaAtualizadaNormalizada = "XYZ9876";

    public const string MarcaPadrao = "Toyota";
    public const string ModeloPadrao = "Corolla";
    public const int AnoPadrao = 2020;

    public const string MarcaAtualizada = "Honda";
    public const string ModeloAtualizado = "Civic";
    public const int AnoAtualizado = 2022;

    public static Placa CriarPlacaPadrao()
    {
        return Placa.Criar(PlacaPadrao);
    }

    public static Placa CriarPlacaAtualizada()
    {
        return Placa.Criar(PlacaAtualizada);
    }

    public static Veiculo CriarVeiculoPadrao(Guid? clienteId = null)
    {
        return Veiculo.Criar(clienteId ?? Guid.NewGuid(), CriarPlacaPadrao(), MarcaPadrao, ModeloPadrao, AnoPadrao);
    }
}