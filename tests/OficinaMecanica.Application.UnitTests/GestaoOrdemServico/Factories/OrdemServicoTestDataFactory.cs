using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AguardarAprovacaoOrcamento;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DetalharOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.EntregarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;

internal static class OrdemServicoTestDataFactory
{
    public const int NumeroPadrao = 1;
    public const decimal ValorServicoPadrao = 150m;
    public const decimal ValorPecaInsumoPadrao = 45m;
    public const int QuantidadePecaInsumoPadrao = 2;

    public const string StatusRecebida = "RECEBIDA";
    public const string StatusEmDiagnostico = "EM_DIAGNOSTICO";
    public const string StatusAguardandoAprovacao = "AGUARDANDO_APROVACAO";
    public const string StatusEmExecucao = "EM_EXECUCAO";
    public const string StatusFinalizada = "FINALIZADA";
    public const string StatusEntregue = "ENTREGUE";
    public const string StatusCancelada = "CANCELADA";

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;
    public const int SegundaPagina = 2;

    public const MotivoCancelamentoOrdemServico MotivoCancelamentoPadrao =
        MotivoCancelamentoOrdemServico.ReprovacaoOrcamento;

    public static OrdemServico CriarOrdemServicoRecebida()
    {
        return OrdemServico.Abrir(
            NumeroPadrao,
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoRecebida();

        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComServico()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();

        ordemServico.DefinirServico(
            Guid.NewGuid(),
            ValorServicoPadrao);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmDiagnosticoComOrcamentoCompleto()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(
            Guid.NewGuid(),
            QuantidadePecaInsumoPadrao,
            ValorPecaInsumoPadrao);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.AguardarAprovacao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoPendente()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.IniciarExecucao();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoPendente();

        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.IniciarExecucaoServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoFinalizado()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoEmExecucao();

        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.FinalizarServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoFinalizada()
    {
        var ordemServico = CriarOrdemServicoEmExecucaoComServicoFinalizado();

        ordemServico.Finalizar();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEntregue()
    {
        var ordemServico = CriarOrdemServicoFinalizada();

        ordemServico.Entregar();

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoCancelada()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.Cancelar(MotivoCancelamentoPadrao);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
        Guid pecaInsumoCatalogoId,
        int quantidade = QuantidadePecaInsumoPadrao)
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(
            pecaInsumoCatalogoId,
            quantidade,
            ValorPecaInsumoPadrao);

        ordemServico.AguardarAprovacao();
        ordemServico.IniciarExecucao();

        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        return ordemServico;
    }

    public static OrdemServico CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(
        Guid pecaInsumoCatalogoId,
        int quantidade = QuantidadePecaInsumoPadrao)
    {
        var ordemServico = CriarOrdemServicoEmDiagnosticoComServico();

        ordemServico.ReservarPecaInsumo(
            pecaInsumoCatalogoId,
            quantidade,
            ValorPecaInsumoPadrao);

        ordemServico.AguardarAprovacao();

        return ordemServico;
    }

    public static AbrirOrdemServicoRequest CriarAbrirOrdemServicoRequestValido(
        Guid? veiculoId = null,
        Guid? mecanicoId = null)
    {
        return new AbrirOrdemServicoRequest(
            veiculoId ?? Guid.NewGuid(),
            mecanicoId ?? Guid.NewGuid());
    }

    public static IniciarDiagnosticoOrdemServicoRequest CriarIniciarDiagnosticoOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new IniciarDiagnosticoOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static DefinirServicosRequest CriarDefinirServicosRequestValido(
        Guid? ordemServicoId = null,
        IReadOnlyCollection<Guid>? servicosCatalogoIds = null)
    {
        return new DefinirServicosRequest(
            ordemServicoId ?? Guid.NewGuid(),
            servicosCatalogoIds ?? new[] { Guid.NewGuid() });
    }

    public static ReservarPecaInsumoRequest CriarReservarPecaInsumoRequestValido(
        Guid? ordemServicoId = null,
        Guid? pecaInsumoCatalogoId = null,
        int quantidade = QuantidadePecaInsumoPadrao)
    {
        return new ReservarPecaInsumoRequest(
            ordemServicoId ?? Guid.NewGuid(),
            new[]
            {
                new PecaInsumoRequest(
                    pecaInsumoCatalogoId ?? Guid.NewGuid(),
                    quantidade)
            });
    }

    public static AguardarAprovacaoOrcamentoRequest CriarAguardarAprovacaoOrcamentoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new AguardarAprovacaoOrcamentoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static IniciarExecucaoOrdemServicoRequest CriarIniciarExecucaoOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new IniciarExecucaoOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static IniciarExecucaoServicoRequest CriarIniciarExecucaoServicoRequestValido(
        Guid? ordemServicoId = null,
        Guid? servicoId = null)
    {
        return new IniciarExecucaoServicoRequest(
            ordemServicoId ?? Guid.NewGuid(),
            servicoId ?? Guid.NewGuid());
    }

    public static FinalizarServicoRequest CriarFinalizarServicoRequestValido(
        Guid? ordemServicoId = null,
        Guid? servicoId = null)
    {
        return new FinalizarServicoRequest(
            ordemServicoId ?? Guid.NewGuid(),
            servicoId ?? Guid.NewGuid());
    }

    public static FinalizarOrdemServicoRequest CriarFinalizarOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new FinalizarOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static EntregarOrdemServicoRequest CriarEntregarOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new EntregarOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static CancelarOrdemServicoRequest CriarCancelarOrdemServicoRequestValido(
        Guid? ordemServicoId = null,
        MotivoCancelamentoOrdemServico motivo = MotivoCancelamentoPadrao)
    {
        return new CancelarOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid(),
            motivo);
    }

    public static ConsultarStatusOrdemServicoRequest CriarConsultarStatusOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new ConsultarStatusOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static DetalharOrdemServicoRequest CriarDetalharOrdemServicoRequestValido(
        Guid? ordemServicoId = null)
    {
        return new DetalharOrdemServicoRequest(
            ordemServicoId ?? Guid.NewGuid());
    }

    public static ListarOrdensServicoRequest CriarListarOrdensServicoRequestValido(
        int pagina = PaginaPadrao,
        int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarOrdensServicoRequest(
            pagina,
            tamanhoPagina);
    }
}