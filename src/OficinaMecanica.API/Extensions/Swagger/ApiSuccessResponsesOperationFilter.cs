using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal sealed class ApiSuccessResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses ??= [];
        operation.Responses.Remove(StatusCodes.Status200OK.ToString(CultureInfo.InvariantCulture));

        var statusCode = ObterStatusCodeSucesso(context.MethodInfo);
        var statusCodeTexto = statusCode.ToString(CultureInfo.InvariantCulture);

        operation.Responses[statusCodeTexto] = statusCode == StatusCodes.Status204NoContent
            ? OpenApiSuccessResponseFactory.CreateNoContent(ApiResponseMessages.RecursoRemovidoComSucesso)
            : OpenApiSuccessResponseFactory.Create(
                context,
                ObterDescricaoSucesso(statusCode),
                ObterTipoResponse(context.MethodInfo));
    }

    private static int ObterStatusCodeSucesso(MethodInfo methodInfo)
    {
        if (methodInfo.GetCustomAttribute<HttpDeleteAttribute>() is not null)
        {
            return StatusCodes.Status204NoContent;
        }

        if (methodInfo.GetCustomAttribute<HttpPostAttribute>() is not null
            && DeveRetornarCreated(methodInfo))
        {
            return StatusCodes.Status201Created;
        }

        return StatusCodes.Status200OK;
    }

    private static bool DeveRetornarCreated(MethodInfo methodInfo)
    {
        return methodInfo.Name.StartsWith("Cadastrar", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("RegistrarEntrada", StringComparison.Ordinal)
            || methodInfo.Name.StartsWith("Abrir", StringComparison.Ordinal);
    }

    private static string ObterDescricaoSucesso(int statusCode)
    {
        return statusCode == StatusCodes.Status201Created
            ? ApiResponseMessages.RecursoCriadoComSucesso
            : ApiResponseMessages.OperacaoExecutadaComSucesso;
    }

    private static Type? ObterTipoResponse(MethodInfo methodInfo)
    {
        var useCaseType = methodInfo
            .GetParameters()
            .FirstOrDefault(parameter => parameter.GetCustomAttribute<FromServicesAttribute>() is not null)
            ?.ParameterType;

        var executeAsyncReturnType = useCaseType?
            .GetMethod("ExecuteAsync")
            ?.ReturnType;

        if (executeAsyncReturnType is null
            || !executeAsyncReturnType.IsGenericType
            || executeAsyncReturnType.GetGenericTypeDefinition() != typeof(Task<>))
        {
            return null;
        }

        var resultType = executeAsyncReturnType.GetGenericArguments()[0];

        if (!resultType.IsGenericType
            || resultType.GetGenericTypeDefinition() != typeof(Result<>))
        {
            return null;
        }

        return resultType.GetGenericArguments()[0];
    }
}
