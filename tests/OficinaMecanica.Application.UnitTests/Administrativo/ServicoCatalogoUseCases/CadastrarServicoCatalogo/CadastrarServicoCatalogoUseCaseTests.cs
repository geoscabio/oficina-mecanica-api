using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;
using OficinaMecanica.Application.Common;
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
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarServicoCatalogoRequest("Troca de óleo", 150m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Descricao.Should().Be(request.Descricao);
        resultado.Valor.Valor.Should().Be(request.Valor);
        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<ServicoCatalogo>(servico =>
                    servico.Descricao == request.Descricao
                    && servico.Valor == request.Valor),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_DescricaoVazia_Quando_CadastrarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarServicoCatalogoRequest(string.Empty, 150m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<ServicoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ValorInvalido_Quando_CadastrarServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);
        var request = new CadastrarServicoCatalogoRequest("Troca de óleo", 0m);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
        repository.Verify(
            repo => repo.AdicionarAsync(It.IsAny<ServicoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CadastrarServicoCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new CadastrarServicoCatalogoUseCase(
            repository.Object,
            new CadastrarServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
