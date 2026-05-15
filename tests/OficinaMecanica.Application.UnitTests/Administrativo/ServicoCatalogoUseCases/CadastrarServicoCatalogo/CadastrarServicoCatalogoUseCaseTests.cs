using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;

public class CadastrarServicoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_CadastrarServicoCatalogo_Entao_DeveCadastrarServicoCatalogo()
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarCadastrarServicoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Descricao.Should().Be(ServicoCatalogoTestDataFactory.DescricaoPadrao);
        resultado.Valor.Valor.Should().Be(ServicoCatalogoTestDataFactory.ValorPadrao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<ServicoCatalogo>(servico =>
                    servico.Descricao == ServicoCatalogoTestDataFactory.DescricaoPadrao
                    && servico.Valor == ServicoCatalogoTestDataFactory.ValorPadrao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DescricaoInvalida_Quando_CadastrarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao(
        string descricao)
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarCadastrarServicoCatalogoRequestValido(
            descricao: descricao);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<ServicoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_CadastrarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = CriarRepository();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarCadastrarServicoCatalogoRequestValido(
            valor: 0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<ServicoCatalogo>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarRepository()
    {
        return new Mock<IServicoCatalogoRepository>();
    }

    private static CadastrarServicoCatalogoUseCase CriarUseCase(
        Mock<IServicoCatalogoRepository> repository)
    {
        return new CadastrarServicoCatalogoUseCase(
            repository.Object,
            new CadastrarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}