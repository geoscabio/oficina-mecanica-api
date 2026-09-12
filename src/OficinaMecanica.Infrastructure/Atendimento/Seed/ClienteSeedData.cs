using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Enums;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Atendimento.Seed;

internal static class ClienteSeedData
{
    private static readonly ClienteSeed ClienteMaria = new("52998224725", "Maria Oliveira", "Rua das Oficinas", "100", "Centro", "Sao Paulo", "01001000", "11999999999", "maria.oliveira@email.com", StatusCliente.Ativo);

    private static readonly ClienteSeed ClienteCarlos = new("04252011000110", "Carlos Souza", "Avenida Brasil", "250", "Jardim America", "Santo Andre", "09000000", "11988887777", "carlos.souza@email.com", StatusCliente.Ativo);

    private static readonly ClienteSeed ClienteAna = new("11144477735", "Ana Lima", "Rua das Flores", "45", "Vila Nova", "Sao Paulo", "02020000", "11977776666", "ana.lima@email.com", StatusCliente.Inativo);

    public static Task<Cliente> ObterOuCriarMariaAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ClienteMaria, cancellationToken);
    }

    public static Task<Cliente> ObterOuCriarCarlosAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ClienteCarlos, cancellationToken);
    }

    public static Task<Cliente> ObterOuCriarAnaAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ClienteAna, cancellationToken);
    }

    private static async Task<Cliente> ObterOuCriarAsync(OficinaMecanicaDbContext dbContext, ClienteSeed seed, CancellationToken cancellationToken)
    {
        var clienteExistente = await dbContext.Clientes
            .SingleOrDefaultAsync(cliente => cliente.Documento.Numero == seed.Documento, cancellationToken);

        if (clienteExistente is not null)
        {
            return clienteExistente;
        }

        var cliente = Cliente.Criar(CpfCnpj.Criar(seed.Documento), seed.Nome, Endereco.Criar(seed.Logradouro, seed.Numero, seed.Bairro, seed.Cidade, seed.Cep), Telefone.Criar(seed.Telefone), Email.Criar(seed.Email));

        if (seed.Status == StatusCliente.Inativo)
        {
            cliente.Inativar();
        }

        dbContext.Clientes.Add(cliente);

        return cliente;
    }

    private sealed record ClienteSeed(string Documento, string Nome, string Logradouro, string Numero, string Bairro, string Cidade, string Cep, string Telefone, string Email, StatusCliente Status);
}
