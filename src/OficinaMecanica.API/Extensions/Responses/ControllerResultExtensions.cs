using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.Extensions.Responses;

public static class ControllerResultExtensions
{
    public static IActionResult ToCreatedAtActionResult<T>(this ControllerBase controller, Result<T> result, string actionName, Func<T, object> routeValuesFactory)
    {
        if (!result.Sucesso)
        {
            return controller.ToActionResult(result);
        }

        return controller.CreatedAtAction(actionName, routeValuesFactory(result.Valor!), result.Valor);
    }

    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Sucesso)
        {
            return controller.Ok(result.Valor);
        }

        if (result.Erro is null)
        {
            return controller.BadRequest();
        }

        return controller.StatusCode(result.Erro.Tipo.ToHttpStatusCode(), result.Erro);
    }

    public static IActionResult ToNoContentResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Sucesso)
        {
            return controller.NoContent();
        }

        return controller.ToActionResult(result);
    }
}
