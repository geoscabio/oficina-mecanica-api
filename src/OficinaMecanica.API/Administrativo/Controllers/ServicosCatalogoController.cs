using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Administrativo.Requests;
using OficinaMecanica.API.Common;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarServicoCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;

namespace OficinaMecanica.API.Administrativo.Controllers;

[ApiController]
[Route("api/v1/administrativo/servicos-catalogo")]
public sealed class ServicosCatalogoController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Cadastrar(
        [FromServices] CadastrarServicoCatalogoUseCase useCase,
        [FromBody] CadastrarServicoCatalogoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(Consultar),
            servico => new { servicoCatalogoId = servico.Id });
    }

    [HttpGet("{servicoCatalogoId:guid}")]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarServicoCatalogoUseCase useCase,
        Guid servicoCatalogoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConsultarServicoCatalogoRequest(servicoCatalogoId),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromServices] ListarServicosCatalogoUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(
            new ListarServicosCatalogoRequest(pagina, tamanhoPagina),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{servicoCatalogoId:guid}")]
    public async Task<IActionResult> Atualizar(
        [FromServices] AtualizarServicoCatalogoUseCase useCase,
        Guid servicoCatalogoId,
        [FromBody] AtualizarServicoCatalogoApiRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new AtualizarServicoCatalogoRequest(servicoCatalogoId, request.Descricao, request.Valor),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{servicoCatalogoId:guid}")]
    public async Task<IActionResult> Remover(
        [FromServices] RemoverServicoCatalogoUseCase useCase,
        Guid servicoCatalogoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new RemoverServicoCatalogoRequest(servicoCatalogoId),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
