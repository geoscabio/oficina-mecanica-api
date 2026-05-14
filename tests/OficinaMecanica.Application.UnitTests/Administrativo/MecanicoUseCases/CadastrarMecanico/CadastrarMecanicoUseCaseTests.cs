using FluentAssertions;
using Moq;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.MecanicoUseCases.CadastrarMecanico;

public class CadastrarMecanicoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_CadastrarMecanico_Entao_DeveCadastrarMecanico()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);
        var request = MecanicoTestDataFactory.CriarCadastrarMecanicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Nome.Should().Be(request.Nome);
        resultado.Valor.Funcional.Should().Be(request.Funcional);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<Mecanico>(mecanico =>
                    mecanico.Nome == request.Nome
                    && mecanico.Funcional == request.Funcional),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_CadastrarMecanico_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);
        var request = MecanicoTestDataFactory.CriarCadastrarMecanicoRequestValido(nome: nome);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_FuncionalInvalida_Quando_CadastrarMecanico_Entao_DeveRetornarFalhaDeValidacao(string funcional)
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);
        var request = MecanicoTestDataFactory.CriarCadastrarMecanicoRequestValido(funcional: funcional);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CadastrarMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new CadastrarMecanicoUseCase(
            repository.Object,
            new CadastrarMecanicoValidator(),
            MapperFactory.Criar());
    }
}