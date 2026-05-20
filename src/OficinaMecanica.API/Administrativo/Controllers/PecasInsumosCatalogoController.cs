using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Identidade;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

namespace OficinaMecanica.API.Administrativo.Controllers;

[ApiController]
[Authorize(Roles = PerfisAcesso.Administrador)]
[Route("api/v1/administrativo/pecas-insumos-catalogo")]
public sealed class PecasInsumosCatalogoController : ControllerBase
{
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar([FromServices] CadastrarPecaInsumoCatalogoUseCase useCase, [FromBody] CadastrarPecaInsumoCatalogoRequest request, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(result, nameof(Consultar), pecaInsumo => new { pecaInsumoCatalogoId = pecaInsumo.Id });
    }

    [HttpGet("consultar/{pecaInsumoCatalogoId:guid}")]
    public async Task<IActionResult> Consultar([FromServices] ConsultarPecaInsumoCatalogoUseCase useCase, Guid pecaInsumoCatalogoId, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarPecaInsumoCatalogoRequest(pecaInsumoCatalogoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar([FromServices] ListarPecasInsumosCatalogoUseCase useCase, [FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10, CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarPecasInsumosCatalogoRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{pecaInsumoCatalogoId:guid}/atualizar")]
    public async Task<IActionResult> Atualizar([FromServices] AtualizarPecaInsumoCatalogoUseCase useCase, Guid pecaInsumoCatalogoId, [FromBody] AtualizarPecaInsumoCatalogoRequest request, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request with { PecaInsumoCatalogoId = pecaInsumoCatalogoId }, cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{pecaInsumoCatalogoId:guid}/remover")]
    public async Task<IActionResult> Remover([FromServices] RemoverPecaInsumoCatalogoUseCase useCase, Guid pecaInsumoCatalogoId, CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoverPecaInsumoCatalogoRequest(pecaInsumoCatalogoId), cancellationToken);

        return this.ToNoContentResult(result);
    }
}
