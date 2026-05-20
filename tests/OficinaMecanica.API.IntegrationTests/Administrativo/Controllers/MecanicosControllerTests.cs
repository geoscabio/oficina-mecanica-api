using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class MecanicosControllerTests : ApiIntegrationTestBase
{
    public MecanicosControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_MecanicoValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var cadastro = MecanicoRequestBuilder.Novo().BuildCadastro();

        // Act
        var mecanicoCriado = await PostJsonAsync(
            "/api/v1/administrativo/mecanicos/cadastrar",
            cadastro,
            HttpStatusCode.Created);
        var mecanicoId = ObterGuid(mecanicoCriado, "id");

        var mecanicoConsultado = await GetJsonAsync($"/api/v1/administrativo/mecanicos/consultar/{mecanicoId}");
        var mecanicos = await GetJsonAsync("/api/v1/administrativo/mecanicos/listar");

        var atualizacao = MecanicoRequestBuilder.Novo()
            .ComNome("Joao Mecanico Atualizado")
            .ComFuncional("MEC002")
            .BuildAtualizacao(mecanicoId);
        var mecanicoAtualizado = await PutJsonAsync(
            $"/api/v1/administrativo/mecanicos/{mecanicoId}/atualizar",
            atualizacao);

        await DeleteAsync($"/api/v1/administrativo/mecanicos/{mecanicoId}/remover");
        var consultaAposRemocao = await Client.GetAsync($"/api/v1/administrativo/mecanicos/consultar/{mecanicoId}");

        // Assert
        ObterString(mecanicoConsultado, "nome").Should().Be(cadastro.Nome);
        ListagemDevePossuirItens(mecanicos);
        ObterString(mecanicoAtualizado, "nome").Should().Be(atualizacao.Nome);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

