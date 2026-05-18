using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Administrativo.Requests;
using OficinaMecanica.API.Common;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;

namespace OficinaMecanica.API.Administrativo.Controllers;

[ApiController]
[Route("api/v1/administrativo/mecanicos")]
public sealed class MecanicosController : ControllerBase
{
    [HttpPost]
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

    [HttpGet("{mecanicoId:guid}")]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarMecanicoUseCase useCase,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarMecanicoRequest(mecanicoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromServices] ListarMecanicosUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarMecanicosRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{mecanicoId:guid}")]
    public async Task<IActionResult> Atualizar(
        [FromServices] AtualizarMecanicoUseCase useCase,
        Guid mecanicoId,
        [FromBody] AtualizarMecanicoApiRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new AtualizarMecanicoRequest(mecanicoId, request.Nome, request.Funcional),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{mecanicoId:guid}")]
    public async Task<IActionResult> Remover(
        [FromServices] RemoverMecanicoUseCase useCase,
        Guid mecanicoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoverMecanicoRequest(mecanicoId), cancellationToken);

        return this.ToActionResult(result);
    }
}
