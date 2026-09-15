using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;
using OficinaMecanica.Application.Identidade;

namespace OficinaMecanica.API.Atendimento.Controllers;

[ApiController]
[Authorize(Roles = PerfisAcesso.Cliente)]
[Route("api/v1/clientes/me")]
public sealed class ClientesMeController : ControllerBase
{
    [HttpGet("ordens-servico")]
    public async Task<IActionResult> ListarOrdensServico(
        [FromServices] ListarOrdensServicoUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var clienteIdClaim = User.FindFirst("cliente_id")?.Value;

        if (!Guid.TryParse(clienteIdClaim, out var clienteId) || clienteId == Guid.Empty)
        {
            return Unauthorized(new ErrorResponse(ApiResponseMessages.NaoAutorizado, TipoErro.NaoAutorizado));
        }

        var result = await useCase.ExecuteAsync(
            new ListarOrdensServicoRequest(pagina, tamanhoPagina, clienteId),
            cancellationToken);

        return this.ToActionResult(result);
    }
}
