using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.GestaoEstoque.Builders;

namespace OficinaMecanica.API.IntegrationTests.GestaoEstoque.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class EstoqueControllerTests : ApiIntegrationTestBase
{
    public EstoqueControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_EntradaEstoqueValida_Quando_ConsultarListarEAtualizar_Entao_DevePersistirEstoque()
    {
        // Arrange
        var pecaInsumoCatalogoId = await CadastrarPecaInsumoCatalogoAsync();
        var registroEntrada = EstoqueRequestBuilder.Novo()
            .ComPecaInsumoCatalogoId(pecaInsumoCatalogoId)
            .ComQuantidade(5)
            .BuildRegistroEntrada();

        // Act
        var itemCriado = await PostJsonAsync("/api/v1/gestao-estoque/estoque/registrar-entrada", registroEntrada, HttpStatusCode.Created);
        var itemEstoqueId = ObterGuid(itemCriado, "id");

        var itemConsultado = await GetJsonAsync($"/api/v1/gestao-estoque/estoque/consultar-item/{itemEstoqueId}");
        var itens = await GetJsonAsync("/api/v1/gestao-estoque/estoque/listar-itens");

        var atualizacao = EstoqueRequestBuilder.Novo()
            .ComPecaInsumoCatalogoId(pecaInsumoCatalogoId)
            .ComQuantidade(10)
            .BuildAtualizacao();
        var itemAtualizado = await PutJsonAsync($"/api/v1/gestao-estoque/estoque/{pecaInsumoCatalogoId}/atualizar-quantidade-disponivel", atualizacao);

        // Assert
        ObterGuid(itemConsultado, "id").Should().Be(itemEstoqueId);
        ListagemDevePossuirItens(itens);
        itemAtualizado.GetProperty("quantidadeDisponivel").GetInt32().Should().Be(10);
    }

    private async Task<Guid> CadastrarPecaInsumoCatalogoAsync()
    {
        var response = await PostJsonAsync("/api/v1/administrativo/pecas-insumos-catalogo/cadastrar", PecaInsumoCatalogoRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }
}

