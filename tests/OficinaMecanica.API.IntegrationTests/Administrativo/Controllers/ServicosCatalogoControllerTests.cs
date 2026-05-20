using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class ServicosCatalogoControllerTests : ApiIntegrationTestBase
{
    public ServicosCatalogoControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_ServicoCatalogoValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var cadastro = ServicoCatalogoRequestBuilder.Novo().BuildCadastro();

        // Act
        var servicoCriado = await PostJsonAsync(
            "/api/v1/administrativo/servicos-catalogo/cadastrar",
            cadastro,
            HttpStatusCode.Created);
        var servicoCatalogoId = ObterGuid(servicoCriado, "id");

        var servicoConsultado = await GetJsonAsync(
            $"/api/v1/administrativo/servicos-catalogo/consultar/{servicoCatalogoId}");
        var servicos = await GetJsonAsync("/api/v1/administrativo/servicos-catalogo/listar");

        var atualizacao = ServicoCatalogoRequestBuilder.Novo()
            .ComDescricao("Alinhamento")
            .ComValor(220m)
            .BuildAtualizacao(servicoCatalogoId);
        var servicoAtualizado = await PutJsonAsync(
            $"/api/v1/administrativo/servicos-catalogo/{servicoCatalogoId}/atualizar",
            atualizacao);

        await DeleteAsync($"/api/v1/administrativo/servicos-catalogo/{servicoCatalogoId}/remover");
        var consultaAposRemocao = await Client.GetAsync(
            $"/api/v1/administrativo/servicos-catalogo/consultar/{servicoCatalogoId}");

        // Assert
        ObterString(servicoConsultado, "descricao").Should().Be(cadastro.Descricao);
        ListagemDevePossuirItens(servicos);
        ObterString(servicoAtualizado, "descricao").Should().Be(atualizacao.Descricao);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

