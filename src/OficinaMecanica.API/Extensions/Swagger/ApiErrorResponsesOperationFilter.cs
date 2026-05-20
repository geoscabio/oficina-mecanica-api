using System.Globalization;
using Microsoft.OpenApi;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal sealed class ApiErrorResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        AddErrorResponse(operation, context, TipoErro.Validacao, "Requisição inválida.");
        AddErrorResponse(operation, context, TipoErro.NaoAutorizado, "Não autorizado.");
        AddErrorResponse(operation, context, TipoErro.NaoEncontrado, "Recurso não encontrado.");
        AddErrorResponse(operation, context, TipoErro.RegraNegocio, "Regra de negócio violada.");
        AddErrorResponse(operation, context, TipoErro.ErroInterno, "Erro interno inesperado.");
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
            OpenApiErrorResponseFactory.Create(context, description));
    }
}
