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
    private readonly record struct OrdemSeedInfo(int Numero, Guid VeiculoId, Guid MecanicoId);

    public static async Task SeedAsync(OficinaMecanicaDbContext dbContext, AdministrativoSeedResult administrativo, AtendimentoSeedResult atendimento, EstoqueSeedResult estoqueSeed, CancellationToken cancellationToken)
    {
        var estoque = estoqueSeed.Estoque;

        await CriarOrdemRecebidaAsync(dbContext, new OrdemSeedInfo(9001, atendimento.VeiculoCivic.Id, administrativo.MecanicoPrincipal.Id), cancellationToken);

        await CriarOrdemEmDiagnosticoAsync(dbContext, new OrdemSeedInfo(9002, atendimento.VeiculoOnix.Id, administrativo.MecanicoDiagnostico.Id), cancellationToken);

        await CriarOrdemAguardandoAprovacaoAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaFiltroOleo.Id, new OrdemSeedInfo(9003, atendimento.VeiculoCorolla.Id, administrativo.MecanicoPrincipal.Id), cancellationToken);

        await CriarOrdemEmExecucaoAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.InsumoOleoMotor.Id, new OrdemSeedInfo(9004, atendimento.VeiculoCivic.Id, administrativo.MecanicoDiagnostico.Id), cancellationToken);

        await CriarOrdemFinalizadaAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaPastilhaFreio.Id, new OrdemSeedInfo(9005, atendimento.VeiculoOnix.Id, administrativo.MecanicoPrincipal.Id), cancellationToken);

        await CriarOrdemEntregueAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaPastilhaFreio.Id, new OrdemSeedInfo(9006, atendimento.VeiculoCorolla.Id, administrativo.MecanicoDiagnostico.Id), cancellationToken);

        await CriarOrdemCanceladaAsync(dbContext, estoque, administrativo.ServicoTrocaOleo.Id, administrativo.PecaFiltroOleo.Id, new OrdemSeedInfo(9007, atendimento.VeiculoCivic.Id, administrativo.MecanicoPrincipal.Id), cancellationToken);
    }

    private static async Task CriarOrdemRecebidaAsync(OficinaMecanicaDbContext dbContext, OrdemSeedInfo ordem, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        dbContext.OrdensServico.Add(OrdemServico.Abrir(ordem.Numero, ordem.VeiculoId, ordem.MecanicoId));
    }

    private static async Task CriarOrdemEmDiagnosticoAsync(OficinaMecanicaDbContext dbContext, OrdemSeedInfo ordem, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = OrdemServico.Abrir(ordem.Numero, ordem.VeiculoId, ordem.MecanicoId);
        ordemServico.IniciarDiagnostico();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemAguardandoAprovacaoAsync(
        OficinaMecanicaDbContext dbContext,
        Estoque estoque,
        Guid servicoCatalogoId,
        Guid pecaInsumoCatalogoId,
        OrdemSeedInfo ordem,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(ordem, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 1, 45m);
        ordemServico.AguardarAprovacao();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemEmExecucaoAsync(
        OficinaMecanicaDbContext dbContext,
        Estoque estoque,
        Guid servicoCatalogoId,
        Guid pecaInsumoCatalogoId,
        OrdemSeedInfo ordem,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(ordem, servicoCatalogoId);
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
        OrdemSeedInfo ordem,
        CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemExecutadaComServicoFinalizado(ordem, estoque, servicoCatalogoId, pecaInsumoCatalogoId);

        BaixarPecasReservadas(ordemServico, estoque);
        ordemServico.Finalizar();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemEntregueAsync(OficinaMecanicaDbContext dbContext, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId, OrdemSeedInfo ordem, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemExecutadaComServicoFinalizado(ordem, estoque, servicoCatalogoId, pecaInsumoCatalogoId);

        BaixarPecasReservadas(ordemServico, estoque);
        ordemServico.Finalizar();
        ordemServico.Entregar();

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static async Task CriarOrdemCanceladaAsync(OficinaMecanicaDbContext dbContext, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId, OrdemSeedInfo ordem, CancellationToken cancellationToken)
    {
        if (await OrdemServicoExisteAsync(dbContext, ordem.Numero, cancellationToken))
        {
            return;
        }

        var ordemServico = CriarOrdemDiagnosticada(ordem, servicoCatalogoId);
        ReservarPecaInsumo(ordemServico, estoque, pecaInsumoCatalogoId, 1, 45m);
        ordemServico.AguardarAprovacao();
        ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);
        EstornarPecasReservadas(ordemServico, estoque);

        dbContext.OrdensServico.Add(ordemServico);
    }

    private static OrdemServico CriarOrdemDiagnosticada(OrdemSeedInfo ordem, Guid servicoCatalogoId)
    {
        var ordemServico = OrdemServico.Abrir(ordem.Numero, ordem.VeiculoId, ordem.MecanicoId);
        ordemServico.IniciarDiagnostico();
        ordemServico.DefinirServico(servicoCatalogoId, 120m);

        return ordemServico;
    }

    private static OrdemServico CriarOrdemExecutadaComServicoFinalizado(OrdemSeedInfo ordem, Estoque estoque, Guid servicoCatalogoId, Guid pecaInsumoCatalogoId)
    {
        var ordemServico = CriarOrdemDiagnosticada(ordem, servicoCatalogoId);
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
