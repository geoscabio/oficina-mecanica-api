using FluentAssertions;
using OficinaMecanica.Domain.OrdemServico.Entities;
using OficinaMecanica.Domain.OrdemServico.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.OrdemServico;

public class PecaInsumoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarPecaInsumo_Entao_DeveRegistrarQuantidadeEValor()
    {
        var catalogoId = Guid.NewGuid();

        var pecaInsumo = PecaInsumo.Criar(catalogoId, 2, 45m);

        pecaInsumo.Id.Should().NotBeEmpty();
        pecaInsumo.PecaInsumoCatalogoId.Should().Be(catalogoId);
        pecaInsumo.Quantidade.Should().Be(2);
        pecaInsumo.ValorUnitario.Should().Be(45m);
        pecaInsumo.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_CriarPecaInsumo_Entao_DeveLancarPecaInsumoInvalidaException()
    {
        var acao = () => PecaInsumo.Criar(Guid.NewGuid(), 0, 45m);

        acao.Should().Throw<PecaInsumoInvalidaException>();
    }

    [Fact]
    public void Dado_ValorUnitarioInvalido_Quando_CriarPecaInsumo_Entao_DeveLancarPecaInsumoInvalidaException()
    {
        var acao = () => PecaInsumo.Criar(Guid.NewGuid(), 2, 0m);

        acao.Should().Throw<PecaInsumoInvalidaException>();
    }
}
