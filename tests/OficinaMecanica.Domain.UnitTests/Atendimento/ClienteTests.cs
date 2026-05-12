using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class ClienteTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarCliente_Entao_DeveRegistrarClienteComIdentidade()
    {
        var documento = CpfCnpj.Criar("529.982.247-25");
        var endereco = new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001000");
        var telefone = Telefone.Criar("(11) 99999-9999");
        var email = Email.Criar("cliente@email.com");

        var cliente = Cliente.Criar(documento, "Maria Silva", endereco, telefone, email);

        cliente.Id.Should().NotBeEmpty();
        cliente.Documento.Should().Be(documento);
        cliente.Nome.Should().Be("Maria Silva");
        cliente.Endereco.Should().Be(endereco);
        cliente.Telefone.Should().Be(telefone);
        cliente.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_CriarCliente_Entao_DeveLancarClienteInvalidoException(string nome)
    {
        var documento = CpfCnpj.Criar("529.982.247-25");
        var endereco = new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001000");
        var telefone = Telefone.Criar("(11) 99999-9999");
        var email = Email.Criar("cliente@email.com");

        var acao = () => Cliente.Criar(documento, nome, endereco, telefone, email);

        acao.Should().Throw<ClienteInvalidoException>();
    }
}
