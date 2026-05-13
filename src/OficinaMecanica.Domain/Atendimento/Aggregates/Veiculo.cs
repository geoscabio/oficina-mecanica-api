using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.Atendimento.Aggregates;

public sealed class Veiculo
{
    private Veiculo(Guid id, Guid clienteId, Placa placa, string marca, string modelo, int ano)
    {
        Id = id;
        ClienteId = clienteId;
        Placa = placa;
        Marca = marca;
        Modelo = modelo;
        Ano = ano;
    }

    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Placa Placa { get; private set; }
    public string Marca { get; private set; }
    public string Modelo { get; private set; }
    public int Ano { get; private set; }

    public static Veiculo Criar(Guid clienteId, Placa placa, string marca, string modelo, int ano)
    {
        if (clienteId == Guid.Empty)
        {
            throw new VeiculoInvalidoException(VeiculoErrorMessages.ClienteObrigatorio);
        }

        if (placa is null)
        {
            throw new VeiculoInvalidoException(VeiculoErrorMessages.PlacaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(marca))
        {
            throw new VeiculoInvalidoException(VeiculoErrorMessages.MarcaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            throw new VeiculoInvalidoException(VeiculoErrorMessages.ModeloObrigatorio);
        }

        if (ano <= 0)
        {
            throw new VeiculoInvalidoException(VeiculoErrorMessages.AnoInvalido);
        }

        return new Veiculo(Guid.NewGuid(), clienteId, placa, marca.Trim(), modelo.Trim(), ano);
    }
}

