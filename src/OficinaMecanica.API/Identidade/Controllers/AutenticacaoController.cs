using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.API.Common;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

namespace OficinaMecanica.API.Identidade.Controllers;

[ApiController]
[Route("api/v1/identidade/autenticacao")]
public sealed class AutenticacaoController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Autenticar(
        [FromServices] AutenticarUsuarioUseCase useCase,
        [FromBody] AutenticarUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request, cancellationToken);

        return this.ToActionResult(result);
    }
}
