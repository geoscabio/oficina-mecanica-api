using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Identidade;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;

namespace OficinaMecanica.API.Atendimento.Controllers;

[ApiController]
[Authorize(Roles = PerfisAcesso.AdministradorAtendente)]
[Route("api/v1/atendimento/clientes")]
public sealed class ClientesController : ControllerBase
{
    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar(
        [FromServices] CadastrarClienteUseCase useCase,
        [FromBody] CadastrarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToCreatedAtActionResult(
            result,
            nameof(Consultar),
            cliente => new { clienteId = cliente.Id });
    }

    [HttpGet("consultar/{clienteId:guid}")]
    public async Task<IActionResult> Consultar(
        [FromServices] ConsultarClienteUseCase useCase,
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ConsultarClienteRequest(clienteId), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("consultar-por-documento/{documento}")]
    public async Task<IActionResult> ConsultarPorDocumento(
        [FromServices] ConsultarClientePorDocumentoUseCase useCase,
        string documento,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ConsultarClientePorDocumentoRequest(documento),
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar(
        [FromServices] ListarClientesUseCase useCase,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await useCase.ExecuteAsync(new ListarClientesRequest(pagina, tamanhoPagina), cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpPut("{clienteId:guid}/atualizar")]
    public async Task<IActionResult> Atualizar(
        [FromServices] AtualizarClienteUseCase useCase,
        Guid clienteId,
        [FromBody] AtualizarClienteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            request with { ClienteId = clienteId },
            cancellationToken);

        return this.ToActionResult(result);
    }

    [HttpDelete("{clienteId:guid}/remover")]
    public async Task<IActionResult> Remover(
        [FromServices] RemoverClienteUseCase useCase,
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new RemoverClienteRequest(clienteId), cancellationToken);

        return this.ToNoContentResult(result);
    }
}
