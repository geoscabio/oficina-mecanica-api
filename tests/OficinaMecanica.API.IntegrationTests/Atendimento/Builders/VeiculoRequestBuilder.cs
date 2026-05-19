using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

namespace OficinaMecanica.API.IntegrationTests.Atendimento.Builders;

public sealed class VeiculoRequestBuilder
{
    private Guid _clienteId = Guid.NewGuid();
    private string _placa = "ABC-1234";
    private string _marca = "Fiat";
    private string _modelo = "Uno";
    private int _ano = 2015;

    public static VeiculoRequestBuilder Novo()
    {
        return new VeiculoRequestBuilder();
    }

    public VeiculoRequestBuilder ComClienteId(Guid clienteId)
    {
        _clienteId = clienteId;

        return this;
    }

    public VeiculoRequestBuilder ComPlaca(string placa)
    {
        _placa = placa;

        return this;
    }

    public VeiculoRequestBuilder ComModelo(string modelo)
    {
        _modelo = modelo;

        return this;
    }

    public CadastrarVeiculoRequest BuildCadastro()
    {
        return new CadastrarVeiculoRequest(_clienteId, _placa, _marca, _modelo, _ano);
    }

    public AtualizarVeiculoRequest BuildAtualizacao(Guid veiculoId)
    {
        return new AtualizarVeiculoRequest(veiculoId, _placa, _marca, _modelo, _ano);
    }
}
