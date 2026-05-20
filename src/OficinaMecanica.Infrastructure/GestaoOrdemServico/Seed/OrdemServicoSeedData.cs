using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Infrastructure.Administrativo.Seed;
using OficinaMecanica.Infrastructure.Atendimento.Seed;
using OficinaMecanica.Infrastructure.GestaoEstoque.Seed;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico.Seed;

internal static class OrdemServicoSeedData
{
    public static async Task SeedAsync(OficinaMecanicaDbContext dbContext, AdministrativoSeedResult administrativo, AtendimentoSeedResult atendimento, EstoqueSeedResult estoqueSeed, CancellationToken cancellationToken)
    {
        var estoque = estoqueSeed.Estoque;

        await CriarOrdemRecebidaAsync(dbContext, 9001, atendimento.VeiculoCivic.Id, administrativo.MecanicoPrincipal.Id, cancellationToken);

        await CriarOrdemEmDiagnosticoAsync(dbContext, 9002, atendimento.VeiculoOnix.Id, administrativo.MecanicoDiagnostico.Id, cancellationToken);

        await CriarOrdemAguardandoAprovacaoAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaFiltroOleo.Id, 9003, atendimento.VeiculoCorolla.Id, administrativo.MecanicoPrincipal.Id, cancellationToken);

        await CriarOrdemEmExecucaoAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.InsumoOleoMotor.Id, 9004, atendimento.VeiculoCivic.Id, administrativo.MecanicoDiagnostico.Id, cancellationToken);

        await CriarOrdemFinalizadaAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaPastilhaFreio.Id, 9005, atendimento.VeiculoOnix.Id, administrativo.MecanicoPrincipal.Id, cancellationToken);

        await CriarOrdemEntregueAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaPastilhaFreio.Id, 9006, atendimento.VeiculoCorolla.Id, administrativo.MecanicoDiagnostico.Id, cancellationToken);

        await CriarOrdemCanceladaAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaFiltroOleo.Id, 9007, atendimento.VeiculoCivic.Id, administrativo.MecanicoPrincipal.Id, cancellationToken);
    }

    private static async Task CriarOrdemRecebidaAsync(OficinaMecanicaDbContext dbContext, int numero, Guid veiculoId, Guid mecanicoId, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        dbContext.OrdensServico.Add(OrdemServico.Abrir(numero, veiculoId, mecanicoId));
    }

    private static async Task CriarOrdemEmDiagnosticoAsync(OficinaMecanicaDbContext dbContext, int numero, Guid veiculoId, Guid mecanicoId, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = OrdemServico.Abrir(numero, veiculoId, mecanicoId);
        ordemServico.IniciarDiagnostico();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemAguardandoAprovacaoAsync(
        OficinaMecanicaDbContext dbContext,
        Estoque estoque,
        Guid servicoCatalogoId,
        Guid pecaInsumoCatalogoId,
        int numero,
        Guid veiculoId,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(numero, veiculoId, mecanicoId, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 1, 45m);
        ordemServico.AguardarAprovacao();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemEmExecucaoAsync(
        OficinaMecanicaDbContext dbContext,
        Estoque estoque,
        Guid servicoCatalogoId,
        Guid pecaInsumoCatalogoId,
        int numero,
        Guid veiculoId,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(numero, veiculoId, mecanicoId, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 2, 65m);
        ordemServico.AguardarAprovacao();
        ordemServico.IniciarExecucao();
        ordemServico.IniciarExecucaoServico(ordemServico.Servicos.First().Id);

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemFinalizadaAsync(
        OficinaMecanicaDbContext dbContext,
        Estoque estoque,
        Guid servicoCatalogoId,
        Guid pecaInsumoCatalogoId,
        int numero,
        Guid veiculoId,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemExecutadaComServicoFinalizado(numero, veiculoId, mecanicoId, estoque, servicoCatalogoId, pecaInsumoCatalogoId);

        BaixarPecasReservadas(ordemServico, estoque);
        ordemServico.Finalizar();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemEntregueAsync(OficinaMecanicaDbContext dbContext, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId, int numero, Guid veiculoId, Guid mecanicoId, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemExecutadaComServicoFinalizado(numero, veiculoId, mecanicoId, estoque, servicoCatalogoId, pecaInsumoCatalogoId);

        BaixarPecasReservadas(ordemServico, estoque);
        ordemServico.Finalizar();
        ordemServico.Entregar();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemCanceladaAsync(OficinaMecanicaDbContext dbContext, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId, int numero, Guid veiculoId, Guid mecanicoId, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(numero, veiculoId, mecanicoId, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 1, 45m);
        ordemServico.AguardarAprovacao();
        ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);
        EstornarPecasReservadas(ordemServico, estoque);

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static OrdemServico CriarOrdemDiagnosticada(int numero, Guid veiculoId, Guid mecanicoId, Guid servicoCatalogoId)
    {
        var ordemServico = OrdemServico.Abrir(numero, veiculoId, mecanicoId);
        ordemServico.IniciarDiagnostico();
        ordemServico.DefinirServico(servicoCatalogoId, 120m);

        return ordemServico;
    }

    private static OrdemServico CriarOrdemExecutadaComServicoFinalizado(int numero, Guid veiculoId, Guid mecanicoId, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId)
    {
        var ordemServico = CriarOrdemDiagnosticada(numero, veiculoId, mecanicoId, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 1, 220m);
        ordemServico.AguardarAprovacao();
        ordemServico.IniciarExecucao();
        ordemServico.IniciarExecucaoServico(ordemServico.Servicos.First().Id);
        ordemServico.FinalizarServico(ordemServico.Servicos.First().Id);

        return ordemServico;
    }

    private static void ReservarPecaInsumo(OrdemServico ordemServico, Estoque estoque, Guid pecaInsumoCatalogoId, int quantidade, decimal valorUnitario)
    {
        ordemServico.ReservarPecaInsumo(pecaInsumoCatalogoId, quantidade, valorUnitario);
        estoque.ReservarItens(pecaInsumoCatalogoId, quantidade);
    }

    private static void BaixarPecasReservadas(OrdemServico ordemServico, Estoque estoque)
    {
        foreach (var pecaInsumo in ordemServico.PecasInsumos)
        {
            estoque.BaixarItens(pecaInsumo.PecaInsumoCatalogoId, pecaInsumo.Quantidade);
        }
    }

    private static void EstornarPecasReservadas(OrdemServico ordemServico, Estoque estoque)
    {
        foreach (var pecaInsumo in ordemServico.PecasInsumos)
        {
            estoque.EstornarItens(pecaInsumo.PecaInsumoCatalogoId, pecaInsumo.Quantidade);
        }
    }

    private static Task<bool> OrdemServicoExisteAsync(OficinaMecanicaDbContext dbContext, int numero, CancellationToken cancellationToken)
    {
        return dbContext.OrdensServico.AnyAsync(ordemServico => ordemServico.Numero == numero, cancellationToken);
    }
}
