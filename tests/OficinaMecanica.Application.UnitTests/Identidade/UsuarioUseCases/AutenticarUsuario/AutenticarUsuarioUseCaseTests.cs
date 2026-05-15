using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;
using OficinaMecanica.Application.Identidade.ValidationMessages;
using OficinaMecanica.Application.UnitTests.Identidade.Factories;

namespace OficinaMecanica.Application.UnitTests.Identidade.UsuarioUseCases.AutenticarUsuario;

public class AutenticarUsuarioUseCaseTests
{
    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_AutenticarUsuario_Entao_DeveRetornarToken()
    {
        // Arrange
        var usuario = AutenticacaoTestDataFactory.CriarUsuarioAutenticadoPadrao();

        var usuarioService = CriarUsuarioService(usuario);

        var tokenService = CriarTokenService("token-jwt");

        var useCase = CriarUseCase(usuarioService, tokenService);

        var request = AutenticacaoTestDataFactory.CriarRequestAutenticacaoValida();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Token.Should().Be("token-jwt");

        resultado.Valor.UsuarioId.Should().Be(usuario.UsuarioId);

        resultado.Valor.Nome.Should().Be(usuario.Nome);

        resultado.Valor.Login.Should().Be(usuario.Login);

        resultado.Valor.Perfil.Should().Be(usuario.Perfil);

        tokenService.Verify(
            service => service.GerarToken(
                usuario.UsuarioId,
                usuario.Nome,
                usuario.Login,
                usuario.Perfil),
            Times.Once);
    }

    [Fact]
    public async Task Dado_CredenciaisInvalidas_Quando_AutenticarUsuario_Entao_DeveRetornarNaoAutorizado()
    {
        // Arrange
        var usuarioService = CriarUsuarioService(null);

        var tokenService = new Mock<ITokenService>();

        var useCase = CriarUseCase(usuarioService, tokenService);

        var request = AutenticacaoTestDataFactory.CriarRequestAutenticacaoValida();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Mensagem.Should().Be(IdentidadeValidationMessages.CredenciaisInvalidas);

        resultado.Erro.Tipo.Should().Be(TipoErro.NaoAutorizado);

        tokenService.Verify(
            service => service.GerarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_LoginVazio_Quando_AutenticarUsuario_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var usuarioService = new Mock<IUsuarioAutenticadoService>();

        var tokenService = new Mock<ITokenService>();

        var useCase = CriarUseCase(usuarioService, tokenService);

        var request = AutenticacaoTestDataFactory
            .CriarRequestAutenticacaoValida() with
        {
            Login = string.Empty
        };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        usuarioService.Verify(
            service => service.AutenticarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_SenhaVazia_Quando_AutenticarUsuario_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var usuarioService = new Mock<IUsuarioAutenticadoService>();

        var tokenService = new Mock<ITokenService>();

        var useCase = CriarUseCase(usuarioService, tokenService);

        var request = AutenticacaoTestDataFactory
            .CriarRequestAutenticacaoValida() with
        {
            Senha = string.Empty
        };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        usuarioService.Verify(
            service => service.AutenticarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static AutenticarUsuarioUseCase CriarUseCase(
        Mock<IUsuarioAutenticadoService> usuarioService,
        Mock<ITokenService> tokenService)
    {
        return new AutenticarUsuarioUseCase(
            usuarioService.Object,
            tokenService.Object,
            new AutenticarUsuarioValidator());
    }

    private static Mock<IUsuarioAutenticadoService> CriarUsuarioService(
        AutenticarUsuarioResponse? usuario)
    {
        var service = new Mock<IUsuarioAutenticadoService>();

        service
            .Setup(service => service.AutenticarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);

        return service;
    }

    private static Mock<ITokenService> CriarTokenService(string token)
    {
        var service = new Mock<ITokenService>();

        service
            .Setup(service => service.GerarToken(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(token);

        return service;
    }
}