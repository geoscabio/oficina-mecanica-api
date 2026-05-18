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
            await WriteErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                new ErrorResponse(exception.Message, TipoErro.RegraNegocio));
        }
        catch
        {
            await WriteErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                new ErrorResponse("Erro interno inesperado.", TipoErro.RegraNegocio));
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        ErrorResponse error)
    {
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(error);
    }
}
