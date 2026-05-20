using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Atendimento.Common;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

namespace OficinaMecanica.API.IntegrationTests.Atendimento.Builders;

public sealed class ClienteRequestBuilder
{
    private string _documento = "529.982.247-25";
    private string _nome = "Maria Cliente";
    private string _telefone = "(11) 99999-9999";
    private string _email = "maria.cliente@email.com";
    private EnderecoRequest _endereco = new("Rua das Oficinas", "100", "Centro", "Sao Paulo", "01001-000");

    public static ClienteRequestBuilder Novo()
    {
        return new ClienteRequestBuilder();
    }

    public ClienteRequestBuilder ComDocumento(string documento)
    {
        _documento = documento;

        return this;
    }

    public ClienteRequestBuilder ComNome(string nome)
    {
        _nome = nome;

        return this;
    }

    public ClienteRequestBuilder ComTelefone(string telefone)
    {
        _telefone = telefone;

        return this;
    }

    public ClienteRequestBuilder ComEmail(string email)
    {
        _email = email;

        return this;
    }

    public CadastrarClienteRequest BuildCadastro()
    {
        return new CadastrarClienteRequest(_documento, _nome, _endereco, _telefone, _email);
    }

    public AtualizarClienteRequest BuildAtualizacao(Guid clienteId)
    {
        return new AtualizarClienteRequest(clienteId, _nome, _endereco, _telefone, _email);
    }
}
