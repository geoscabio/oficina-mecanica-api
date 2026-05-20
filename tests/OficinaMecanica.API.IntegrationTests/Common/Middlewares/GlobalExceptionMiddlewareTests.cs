using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OficinaMecanica.API.Middlewares;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.API.IntegrationTests.Common.Middlewares;

public sealed class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task Dado_DomainException_Quando_MiddlewareCapturar_Entao_DeveRetornarErroDeRegraNegocio()
    {
        // Arrange
        const string mensagem = "Regra de domínio inválida.";
        var context = CriarHttpContext();
        var middleware = new GlobalExceptionMiddleware(_ => throw new DomainException(mensagem));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);

        var erro = await LerErroAsync(context);
        erro.GetProperty("tipo").GetString().Should().Be(nameof(TipoErro.RegraNegocio));
        erro.GetProperty("mensagem").GetString().Should().Be(mensagem);
    }

    [Fact]
    public async Task Dado_ErroInesperado_Quando_MiddlewareCapturar_Entao_DeveRetornarErroInterno()
    {
        // Arrange
        var context = CriarHttpContext();
        var middleware = new GlobalExceptionMiddleware(_ => throw new InvalidOperationException("Falha externa."));

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var erro = await LerErroAsync(context);
        erro.GetProperty("tipo").GetString().Should().Be(nameof(TipoErro.ErroInterno));
        erro.GetProperty("mensagem").GetString().Should().Be(ApiResponseMessages.ErroInternoInesperado);
    }

    private static DefaultHttpContext CriarHttpContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static async Task<JsonElement> LerErroAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;

        using var document = await JsonDocument.ParseAsync(context.Response.Body);

        return document.RootElement.Clone();
    }
}
