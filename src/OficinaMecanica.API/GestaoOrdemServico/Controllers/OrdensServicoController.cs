using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Identidade;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AguardarAprovacaoOrcamento;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarTempoMedioExecucaoServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DetalharOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.EntregarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarTempoMedioExecucaoServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

namespace OficinaMecanica.API.GestaoOrdemServico.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/gestao-ordem-servico")]
public sealed class OrdensServicoController : ControllerBase
{
    [HttpPost("ordens-servico/cadastrar")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
    public async Task<IActionResult> Abrir(
        [FromServices] AbrirOrdemServicoUseCase useCase,
        [FromBody] AbrirOrdemServicoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(Detalhar),
            ordemServico => new { ordemServicoId = ordemServico.Id });
    }

    [HttpGet("ordens-servico/consultar/{ordemServicoId:guid}")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanico)]
    public async Task<IActionResult> Detalhar(
        [FromServices] DetalharOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DetalharOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("ordens-servico/{ordemServicoId:guid}/consultar-status")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanicoCliente)]
    public async Task<IActionResult> ConsultarStatus(
        [FromServices] ConsultarStatusOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarStatusOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("ordens-servico/listar")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanico)]
    public async Task<IActionResult> Listar(
        [FromServices] ListarOrdensServicoUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/iniciar-diagnostico")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> IniciarDiagnostico(
        [FromServices] IniciarDiagnosticoOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new IniciarDiagnosticoOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("ordens-servico/{ordemServicoId:guid}/definir-servicos")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> DefinirServicos(
        [FromServices] DefinirServicosUseCase useCase,
        Guid ordemServicoId,
        [FromBody] DefinirServicosRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { OrdemServicoId = ordemServicoId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/reservar-pecas-insumos")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> ReservarPecasInsumos(
        [FromServices] ReservarPecaInsumoUseCase useCase,
        Guid ordemServicoId,
        [FromBody] ReservarPecaInsumoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { OrdemServicoId = ordemServicoId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/aguardar-aprovacao")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
    public async Task<IActionResult> AguardarAprovacaoOrcamento(
        [FromServices] AguardarAprovacaoOrcamentoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new AguardarAprovacaoOrcamentoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/iniciar-execucao")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
    public async Task<IActionResult> IniciarExecucao(
        [FromServices] IniciarExecucaoOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new IniciarExecucaoOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/servicos/{servicoId:guid}/iniciar-execucao")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> IniciarExecucaoServico(
        [FromServices] IniciarExecucaoServicoUseCase useCase,
        Guid ordemServicoId,
        Guid servicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new IniciarExecucaoServicoRequest(ordemServicoId, servicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/servicos/{servicoId:guid}/finalizar")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> FinalizarServico(
        [FromServices] FinalizarServicoUseCase useCase,
        Guid ordemServicoId,
        Guid servicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new FinalizarServicoRequest(ordemServicoId, servicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/finalizar")]
    [Authorize(Roles = PerfisAcesso.AdministradorMecanico)]
    public async Task<IActionResult> Finalizar(
        [FromServices] FinalizarOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/entregar")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
    public async Task<IActionResult> Entregar(
        [FromServices] EntregarOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new EntregarOrdemServicoRequest(ordemServicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPost("ordens-servico/{ordemServicoId:guid}/cancelar")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanico)]
    public async Task<IActionResult> Cancelar(
        [FromServices] CancelarOrdemServicoUseCase useCase,
        Guid ordemServicoId,
        [FromBody] CancelarOrdemServicoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { OrdemServicoId = ordemServicoId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("tempo-medio-servicos/consultar/{servicoCatalogoId:guid}")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanico)]
    public async Task<IActionResult> ConsultarTempoMedio(
        [FromServices] ConsultarTempoMedioExecucaoServicoUseCase useCase,
        Guid servicoCatalogoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConsultarTempoMedioExecucaoServicoRequest(servicoCatalogoId),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("tempo-medio-servicos/listar")]
    [Authorize(Roles = PerfisAcesso.AdministradorAtendenteMecanico)]
    public async Task<IActionResult> ListarTempoMedio(
        [FromServices] ListarTempoMedioExecucaoServicosUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(
            new ListarTempoMedioExecucaoServicosRequest(pagina, tamanhoPagina),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
