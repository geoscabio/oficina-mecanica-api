using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Builders;

internal static class MecanicoTestDataFactory
{
    public static Mecanico CriarMecanicoPadrao()
    {
        return Mecanico.Criar("Jose Santos", "MEC-001");
    }
}

