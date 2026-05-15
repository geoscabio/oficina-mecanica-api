using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class ClienteTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarCliente_Entao_DeveRegistrarClienteComIdentidade()
    {
        // Arrange
        var documento = ClienteTestDataFactory.CriarDocumentoPadrao();
        var endereco = ClienteTestDataFactory.CriarEnderecoPadrao();
        var telefone = ClienteTestDataFactory.CriarTelefonePadrao();
        var email = ClienteTestDataFactory.CriarEmailPadrao();

        // Act
        var cliente = Cliente.Criar(
            documento,
            ClienteTestDataFactory.NomePadrao,
            endereco,
            telefone,
            email);

        // Assert
        cliente.Id.Should().NotBeEmpty();
        cliente.Documento.Should().Be(documento);
        cliente.Documento.Numero.Should().Be(ClienteTestDataFactory.DocumentoNormalizadoPadrao);
        cliente.Nome.Should().Be(ClienteTestDataFactory.NomePadrao);
        cliente.Endereco.Should().Be(endereco);
        cliente.Telefone.Should().Be(telefone);
        cliente.Telefone.Numero.Should().Be(ClienteTestDataFactory.TelefoneNormalizadoPadrao);
        cliente.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_CriarCliente_Entao_DeveLancarDomainException(string nome)
    {
        // Arrange
        var documento = ClienteTestDataFactory.CriarDocumentoPadrao();
        var endereco = ClienteTestDataFactory.CriarEnderecoPadrao();
        var telefone = ClienteTestDataFactory.CriarTelefonePadrao();
        var email = ClienteTestDataFactory.CriarEmailPadrao();

        // Act
        var acao = () => Cliente.Criar(
            documento,
            nome,
            endereco,
            telefone,
            email);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ClienteErrorMessages.NomeObrigatorio);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarCliente_Entao_DeveAtualizarDados()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var novoEndereco = ClienteTestDataFactory.CriarEnderecoAtualizado();
        var novoTelefone = ClienteTestDataFactory.CriarTelefoneAtualizado();
        var novoEmail = ClienteTestDataFactory.CriarEmailAtualizado();

        // Act
        cliente.Atualizar(
            ClienteTestDataFactory.NomeAtualizado,
            novoEndereco,
            novoTelefone,
            novoEmail);

        // Assert
        cliente.Nome.Should().Be(ClienteTestDataFactory.NomeAtualizado);
        cliente.Endereco.Should().Be(novoEndereco);
        cliente.Telefone.Should().Be(novoTelefone);
        cliente.Telefone.Numero.Should().Be(ClienteTestDataFactory.TelefoneAtualizadoNormalizado);
        cliente.Email.Should().Be(novoEmail);
        cliente.Email.Endereco.Should().Be(ClienteTestDataFactory.EmailAtualizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_AtualizarCliente_Entao_DeveLancarDomainException(string nome)
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        // Act
        var acao = () => cliente.Atualizar(
            nome,
            ClienteTestDataFactory.CriarEnderecoPadrao(),
            ClienteTestDataFactory.CriarTelefonePadrao(),
            ClienteTestDataFactory.CriarEmailPadrao());

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ClienteErrorMessages.NomeObrigatorio);
    }
}