using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.IntegrationTests.Common.Responses;

public sealed class ApiErrorStatusCodeExtensionsTests
{
    [Theory]
    [InlineData(TipoErro.Validacao, StatusCodes.Status400BadRequest)]
    [InlineData(TipoErro.NaoEncontrado, StatusCodes.Status404NotFound)]
    [InlineData(TipoErro.RegraNegocio, StatusCodes.Status422UnprocessableEntity)]
    [InlineData(TipoErro.NaoAutorizado, StatusCodes.Status401Unauthorized)]
    [InlineData(TipoErro.AcessoProibido, StatusCodes.Status403Forbidden)]
    [InlineData(TipoErro.ErroInterno, StatusCodes.Status500InternalServerError)]
    public void Dado_TipoErro_Quando_ConverterParaHttpStatusCode_Entao_DeveRetornarStatusPadronizado(
        TipoErro tipoErro,
        int statusCodeEsperado)
    {
        // Arrange

        // Act
        var statusCode = tipoErro.ToHttpStatusCode();

        // Assert
        statusCode.Should().Be(statusCodeEsperado);
    }
}
