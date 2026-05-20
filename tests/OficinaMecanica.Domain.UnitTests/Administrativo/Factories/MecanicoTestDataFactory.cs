using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

internal static class MecanicoTestDataFactory
{
    public const string NomePadrao = "Jose Santos";
    public const string FuncionalPadrao = "MEC-001";

    public const string NomeAtualizado = "Carlos Silva";
    public const string FuncionalAtualizado = "MEC-002";

    public static Mecanico CriarMecanicoPadrao(string nome = NomePadrao, string funcional = FuncionalPadrao)
    {
        return Mecanico.Criar(nome, funcional);
    }
}