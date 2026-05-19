using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions;
using OficinaMecanica.Application.Identidade;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.RemoverVeiculo;

namespace OficinaMecanica.API.Atendimento.Controllers;

[ApiController]
[Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
[Route("api/v1/atendimento/veiculos")]
public sealed class VeiculosController : ControllerBase
{
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar(
        [FromServices] CadastrarVeiculoUseCase useCase,
        [FromBody] CadastrarVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(Consultar),
            veiculo => new { veiculoId = veiculo.Id });
    }

    [HttpGet("consultar/{veiculoId:guid}")]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarVeiculoUseCase useCase,
        Guid veiculoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarVeiculoRequest(veiculoId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("consultar-por-placa/{placa}")]
    public async Task<IActionResult> ConsultarPorPlaca(
        [FromServices] ConsultarVeiculoPorPlacaUseCase useCase,
        string placa,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest(placa), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar(
        [FromServices] ListarVeiculosUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarVeiculosRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{veiculoId:guid}/atualizar")]
    public async Task<IActionResult> Atualizar(
        [FromServices] AtualizarVeiculoUseCase useCase,
        Guid veiculoId,
        [FromBody] AtualizarVeiculoRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { VeiculoId = veiculoId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{veiculoId:guid}/remover")]
    public async Task<IActionResult> Remover(
        [FromServices] RemoverVeiculoUseCase useCase,
        Guid veiculoId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoverVeiculoRequest(veiculoId), cancellationToken);

        return this.ToActionResult(result);
    }
}
