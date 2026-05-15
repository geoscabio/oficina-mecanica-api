using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Factories;

internal static class MecanicoTestDataFactory
{
    public const string NomePadrao = "Jose Santos";
    public const string FuncionalPadrao = "MEC-001";

    public static Mecanico CriarMecanicoPadrao(
        string nome = NomePadrao,
        string funcional = FuncionalPadrao)
    {
        return Mecanico.Criar(nome, funcional);
    }

    public static CadastrarMecanicoRequest CriarCadastrarMecanicoRequestValido(
        string nome = NomePadrao,
        string funcional = FuncionalPadrao)
    {
        return new CadastrarMecanicoRequest(nome, funcional);
    }
}