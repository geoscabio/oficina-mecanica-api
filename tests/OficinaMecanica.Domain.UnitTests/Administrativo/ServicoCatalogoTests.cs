using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class ServicoCatalogoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServicoCatalogo_Entao_DeveRegistrarServicoComValor()
    {
        var servico = ServicoCatalogo.Criar("Troca de oleo", 120m);

        servico.Id.Should().NotBeEmpty();
        servico.Descricao.Should().Be("Troca de oleo");
        servico.Valor.Should().Be(120m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarServicoCatalogo_Entao_DeveLancarServicoCatalogoInvalidoException(string descricao)
    {
        var acao = () => ServicoCatalogo.Criar(descricao, 120m);

        acao.Should().Throw<ServicoCatalogoInvalidoException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarServicoCatalogo_Entao_DeveLancarServicoCatalogoInvalidoException(decimal valor)
    {
        var acao = () => ServicoCatalogo.Criar("Troca de oleo", valor);

        acao.Should().Throw<ServicoCatalogoInvalidoException>();
    }
}
