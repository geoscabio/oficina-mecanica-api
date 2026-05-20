using System.Globalization;
using Microsoft.OpenApi;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal sealed class ApiErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AddErrorResponse(operation, context, TipoErro.Validacao, ApiResponseMessages.RequisicaoInvalida);
        AddErrorResponse(operation, context, TipoErro.NaoAutorizado, ApiResponseMessages.NaoAutorizado);
        AddErrorResponse(operation, context, TipoErro.NaoEncontrado, ApiResponseMessages.RecursoNaoEncontrado);
        AddErrorResponse(operation, context, TipoErro.RegraNegocio, ApiResponseMessages.RegraNegocioViolada);
        AddErrorResponse(operation, context, TipoErro.ErroInterno, ApiResponseMessages.ErroInternoInesperado);
    }

    private static void AddErrorResponse(
        OpenApiOperation operation,
        OperationFilterContext context,
        TipoErro tipoErro,
        string description)
    {
        operation.Responses ??= [];

        var statusCode = tipoErro
            .ToHttpStatusCode()
            .ToString(CultureInfo.InvariantCulture);

        operation.Responses.TryAdd(
            statusCode,
            OpenApiErrorResponseFactory.Create(context, description, tipoErro));
    }
}
