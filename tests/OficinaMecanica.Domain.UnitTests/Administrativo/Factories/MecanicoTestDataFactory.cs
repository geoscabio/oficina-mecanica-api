using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

internal static class MecanicoTestDataFactory
{
    public const string NomePadrao = "Joao Pereira";
    public const string FuncionalPadrao = "Suspensao e freios";

    public static Mecanico CriarMecanicoPadrao()
    {
        return Mecanico.Criar(NomePadrao, FuncionalPadrao);
    }
}
