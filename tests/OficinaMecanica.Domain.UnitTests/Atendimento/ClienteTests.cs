using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Atendimento.Builders;

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
        cliente.Nome.Should().Be(ClienteTestDataFactory.NomePadrao);
        cliente.Endereco.Should().Be(endereco);
        cliente.Telefone.Should().Be(telefone);
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
        var acao = () => Cliente.Criar(documento, nome, endereco, telefone, email);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(ClienteErrorMessages.NomeObrigatorio);
    }
}
