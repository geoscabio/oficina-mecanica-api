using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.API.IntegrationTests.Administrativo.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class PecasInsumosCatalogoControllerTests : ApiIntegrationTestBase
{
    public PecasInsumosCatalogoControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_PecaInsumoCatalogoValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var cadastro = PecaInsumoCatalogoRequestBuilder.Novo().BuildCadastro();

        // Act
        var pecaCriada = await PostJsonAsync("/api/v1/administrativo/pecas-insumos-catalogo/cadastrar", cadastro, HttpStatusCode.Created);
        var pecaInsumoCatalogoId = ObterGuid(pecaCriada, "id");

        var pecaConsultada = await GetJsonAsync($"/api/v1/administrativo/pecas-insumos-catalogo/consultar/{pecaInsumoCatalogoId}");
        var pecas = await GetJsonAsync("/api/v1/administrativo/pecas-insumos-catalogo/listar");

        var atualizacao = PecaInsumoCatalogoRequestBuilder.Novo()
            .ComDescricao("Aditivo de radiador")
            .ComTipo(TipoPecaInsumo.INSUMO)
            .ComValor(35m)
            .BuildAtualizacao(pecaInsumoCatalogoId);
        var pecaAtualizada = await PutJsonAsync($"/api/v1/administrativo/pecas-insumos-catalogo/{pecaInsumoCatalogoId}/atualizar", atualizacao);

        await DeleteAsync($"/api/v1/administrativo/pecas-insumos-catalogo/{pecaInsumoCatalogoId}/remover");
        var consultaAposRemocao = await Client.GetAsync($"/api/v1/administrativo/pecas-insumos-catalogo/consultar/{pecaInsumoCatalogoId}");

        // Assert
        ObterString(pecaConsultada, "descricao").Should().Be(cadastro.Descricao);
        ListagemDevePossuirItens(pecas);
        ObterString(pecaAtualizada, "descricao").Should().Be(atualizacao.Descricao);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

