using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.Extensions.Responses;

public static class ApiErrorStatusCodeExtensions
{
    public static int ToHttpStatusCode(this TipoErro tipoErro)
    {
        return tipoErro switch
        {
            TipoErro.Validacao => StatusCodes.Status400BadRequest,
            TipoErro.NaoEncontrado => StatusCodes.Status404NotFound,
            TipoErro.RegraNegocio => StatusCodes.Status422UnprocessableEntity,
            TipoErro.NaoAutorizado => StatusCodes.Status401Unauthorized,
            TipoErro.ErroInterno => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
