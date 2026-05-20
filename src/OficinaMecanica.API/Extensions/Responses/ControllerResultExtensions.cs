using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.Extensions.Responses;

public static class ControllerResultExtensions
{
    public static IActionResult ToCreatedAtActionResult<T>(
        this ControllerBase controller,
        Result<T> result,
        string actionName,
        Func<T, object> routeValuesFactory)
    {
        if (!result.Sucesso)
        {
            return controller.ToActionResult(result);
        }

        return controller.CreatedAtAction(
            actionName,
            routeValuesFactory(result.Valor!),
            result.Valor);
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Sucesso)
        {
            return controller.Ok(result.Valor);
        }

        return result.Erro?.Tipo switch
        {
            TipoErro.Validacao => controller.BadRequest(result.Erro),
            TipoErro.NaoEncontrado => controller.NotFound(result.Erro),
            TipoErro.RegraNegocio => controller.Conflict(result.Erro),
            TipoErro.NaoAutorizado => controller.Unauthorized(result.Erro),
            TipoErro.ErroInterno => controller.StatusCode(StatusCodes.Status500InternalServerError, result.Erro),
            _ => controller.BadRequest(result.Erro)
        };
    }
}
