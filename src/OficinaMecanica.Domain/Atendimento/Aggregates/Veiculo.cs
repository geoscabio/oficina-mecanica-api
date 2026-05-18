using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.Atendimento.Aggregates;

public sealed class Veiculo
{
    private Veiculo()
    {
        Placa = null!;
        Marca = string.Empty;
        Modelo = string.Empty;
    }

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
            throw new DomainException(VeiculoErrorMessages.ClienteObrigatorio);
        }

        if (placa is null)
        {
            throw new DomainException(VeiculoErrorMessages.PlacaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(marca))
        {
            throw new DomainException(VeiculoErrorMessages.MarcaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            throw new DomainException(VeiculoErrorMessages.ModeloObrigatorio);
        }

        if (ano <= 0)
        {
            throw new DomainException(VeiculoErrorMessages.AnoInvalido);
        }

        return new Veiculo(Guid.NewGuid(), clienteId, placa, marca.Trim(), modelo.Trim(), ano);
    }

    public void Atualizar(Placa placa, string marca, string modelo, int ano)
    {
        if (placa is null)
        {
            throw new DomainException(VeiculoErrorMessages.PlacaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(marca))
        {
            throw new DomainException(VeiculoErrorMessages.MarcaObrigatoria);
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            throw new DomainException(VeiculoErrorMessages.ModeloObrigatorio);
        }

        if (ano <= 0)
        {
            throw new DomainException(VeiculoErrorMessages.AnoInvalido);
        }

        Placa = placa;
        Marca = marca.Trim();
        Modelo = modelo.Trim();
        Ano = ano;
    }
}

