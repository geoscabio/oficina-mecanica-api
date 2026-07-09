using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Common.Health;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class HealthCheckTests : ApiIntegrationTestBase
{
    public HealthCheckTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    [RequiresDockerFact]
    public async Task Dado_RequisicaoHealth_Quando_VerificarStatus_Entao_DeveRetornarHealthy()
    {
        var response = await Client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();

        content.Should().Be("Healthy");
    }
}
