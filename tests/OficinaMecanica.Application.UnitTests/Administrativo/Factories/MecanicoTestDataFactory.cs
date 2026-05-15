using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Factories;

internal static class MecanicoTestDataFactory
{
    public const string NomePadrao = "Jose Santos";
    public const string FuncionalPadrao = "MEC-001";

    public const string NomeAtualizado = "Carlos Silva";
    public const string FuncionalAtualizado = "MEC-002";

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

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
        return new CadastrarMecanicoRequest(
            nome,
            funcional);
    }

    public static AtualizarMecanicoRequest CriarAtualizarMecanicoRequestValido(
        Guid? mecanicoId = null,
        string nome = NomeAtualizado,
        string funcional = FuncionalAtualizado)
    {
        return new AtualizarMecanicoRequest(
            mecanicoId ?? Guid.NewGuid(),
            nome,
            funcional);
    }

    public static ConsultarMecanicoRequest CriarConsultarMecanicoRequestValido(
        Guid? mecanicoId = null)
    {
        return new ConsultarMecanicoRequest(
            mecanicoId ?? Guid.NewGuid());
    }

    public static ListarMecanicosRequest CriarListarMecanicosRequestValido(
        int pagina = PaginaPadrao,
        int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarMecanicosRequest(
            pagina,
            tamanhoPagina);
    }

    public static RemoverMecanicoRequest CriarRemoverMecanicoRequestValido(
        Guid? mecanicoId = null)
    {
        return new RemoverMecanicoRequest(
            mecanicoId ?? Guid.NewGuid());
    }
}