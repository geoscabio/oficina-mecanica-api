using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

internal static class ClienteTestDataFactory
{
    public const string NomePadrao = "Maria Silva";

    public static CpfCnpj CriarDocumentoPadrao()
    {
        return CpfCnpj.Criar("529.982.247-25");
    }

    public static Endereco CriarEnderecoPadrao()
    {
        return new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001000");
    }

    public static Telefone CriarTelefonePadrao()
    {
        return Telefone.Criar("(11) 99999-9999");
    }

    public static Email CriarEmailPadrao()
    {
        return Email.Criar("cliente@email.com");
    }

    public static Cliente CriarClientePadrao()
    {
        return Cliente.Criar(
            CriarDocumentoPadrao(),
            NomePadrao,
            CriarEnderecoPadrao(),
            CriarTelefonePadrao(),
            CriarEmailPadrao());
    }
}
