using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Atendimento.Seed;

internal static class VeiculoSeedData
{
    private static readonly VeiculoSeed VeiculoCivic = new("ABC1234", "Honda", "Civic", 2020);
    private static readonly VeiculoSeed VeiculoOnix = new("BRA2E19", "Chevrolet", "Onix", 2022);
    private static readonly VeiculoSeed VeiculoCorolla = new("FIQ1A23", "Toyota", "Corolla", 2021);

    public static Task<Veiculo> ObterOuCriarCivicAsync(OficinaMecanicaDbContext dbContext, Guid clienteId, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, clienteId, VeiculoCivic, cancellationToken);
    }

    public static Task<Veiculo> ObterOuCriarOnixAsync(OficinaMecanicaDbContext dbContext, Guid clienteId, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, clienteId, VeiculoOnix, cancellationToken);
    }

    public static Task<Veiculo> ObterOuCriarCorollaAsync(OficinaMecanicaDbContext dbContext, Guid clienteId, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, clienteId, VeiculoCorolla, cancellationToken);
    }

    private static async Task<Veiculo> ObterOuCriarAsync(OficinaMecanicaDbContext dbContext, Guid clienteId, VeiculoSeed seed, CancellationToken cancellationToken)
    {
        var placa = Placa.Criar(seed.Placa);
        var veiculoExistente = await dbContext.Veiculos
            .SingleOrDefaultAsync(veiculo => veiculo.Placa.NumeroPlaca == placa.NumeroPlaca, cancellationToken);

        if (veiculoExistente is not null)
        {
            return veiculoExistente;
        }

        var veiculo = Veiculo.Criar(clienteId, placa, seed.Marca, seed.Modelo, seed.Ano);
        dbContext.Veiculos.Add(veiculo);

        return veiculo;
    }

    private sealed record VeiculoSeed(string Placa, string Marca, string Modelo, int Ano);
}
