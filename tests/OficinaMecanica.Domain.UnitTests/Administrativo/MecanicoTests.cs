using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class MecanicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarMecanico_Entao_DeveRegistrarMecanicoComIdentidade()
    {
        var mecanico = Mecanico.Criar("Joao Pereira", "Suspensao e freios");

        mecanico.Id.Should().NotBeEmpty();
        mecanico.Nome.Should().Be("Joao Pereira");
        mecanico.Funcional.Should().Be("Suspensao e freios");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_CriarMecanico_Entao_DeveLancarMecanicoInvalidoException(string nome)
    {
        var acao = () => Mecanico.Criar(nome, "Suspensao e freios");

        acao.Should().Throw<MecanicoInvalidoException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_CriarMecanico_Entao_DeveLancarMecanicoInvalidoException(string funcional)
    {
        var acao = () => Mecanico.Criar("Joao Pereira", funcional);

        acao.Should().Throw<MecanicoInvalidoException>();
    }
}
