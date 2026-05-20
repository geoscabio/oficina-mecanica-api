using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Identidade;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;

namespace OficinaMecanica.API.Administrativo.Controllers;

[ApiController]
[Authorize(Roles = PerfisAcesso.Administrador)]
[Route("api/v1/administrativo/mecanicos")]
public sealed class MecanicosController : ControllerBase
{
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar(
        [FromServices] CadastrarMecanicoUseCase useCase,
        [FromBody] CadastrarMecanicoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(Consultar),
            mecanico => new { mecanicoId = mecanico.Id });
    }

    [HttpGet("consultar/{mecanicoId:guid}")]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarMecanicoUseCase useCase,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarMecanicoRequest(mecanicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar(
        [FromServices] ListarMecanicosUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarMecanicosRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{mecanicoId:guid}/atualizar")]
    public async Task<IActionResult> Atualizar(
        [FromServices] AtualizarMecanicoUseCase useCase,
        Guid mecanicoId,
        [FromBody] AtualizarMecanicoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { MecanicoId = mecanicoId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{mecanicoId:guid}/remover")]
    public async Task<IActionResult> Remover(
        [FromServices] RemoverMecanicoUseCase useCase,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoverMecanicoRequest(mecanicoId), cancellationToken);

        return this.ToNoContentResult(result);
    }
}
