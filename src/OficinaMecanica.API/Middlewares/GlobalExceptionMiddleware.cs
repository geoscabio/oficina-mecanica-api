using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.API.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteErrorAsync(context, TipoErro.RegraNegocio.ToHttpStatusCode(), new ErrorResponse(exception.Message, TipoErro.RegraNegocio));
        }
        catch
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteErrorAsync(context, TipoErro.ErroInterno.ToHttpStatusCode(), new ErrorResponse(ApiResponseMessages.ErroInternoInesperado, TipoErro.ErroInterno));
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, ErrorResponse error)
    {
        context.Response.StatusCode = statusCode;

        await context.Response.WriteApiErrorResponseAsJsonAsync(error);
    }
}
