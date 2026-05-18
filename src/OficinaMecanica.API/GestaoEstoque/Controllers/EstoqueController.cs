using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

namespace OficinaMecanica.API.GestaoEstoque.Controllers;

[ApiController]
[Route("api/v1/gestao-estoque/estoque")]
public sealed class EstoqueController : ControllerBase
{
    [HttpPost("registrar-entrada")]
    public async Task<IActionResult> RegistrarEntrada(
        [FromServices] RegistrarEntradaEstoqueUseCase useCase,
        [FromBody] RegistrarEntradaEstoqueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(ConsultarItem),
            itemEstoque => new { itemEstoqueId = itemEstoque.Id });
    }

    [HttpGet("itens/{itemEstoqueId:guid}")]
    public async Task<IActionResult> ConsultarItem(
        [FromServices] ConsultarItemEstoqueUseCase useCase,
        Guid itemEstoqueId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarItemEstoqueRequest(itemEstoqueId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("itens")]
    public async Task<IActionResult> ListarItens(
        [FromServices] ListarItensEstoqueUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarItensEstoqueRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("itens/{pecaInsumoCatalogoId:guid}/quantidade-disponivel")]
    public async Task<IActionResult> AtualizarQuantidadeDisponivel(
        [FromServices] AtualizarEstoqueUseCase useCase,
        Guid pecaInsumoCatalogoId,
        [FromBody] AtualizarEstoqueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { PecaInsumoCatalogoId = pecaInsumoCatalogoId },
            cancellationToken);

        return this.ToActionResult(result);
    }
}
