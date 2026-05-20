using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

internal static class ClienteTestDataFactory
{
    public const string DocumentoPadrao = "529.982.247-25";
    public const string DocumentoNormalizadoPadrao = "52998224725";

    public const string CnpjPadrao = "04.252.011/0001-10";
    public const string CnpjNormalizadoPadrao = "04252011000110";

    public const string NomePadrao = "Maria Silva";
    public const string NomeAtualizado = "Cliente Atualizado";

    public const string LogradouroPadrao = "Rua A";
    public const string NumeroPadrao = "100";
    public const string BairroPadrao = "Centro";
    public const string CidadePadrao = "Sao Paulo";
    public const string CepPadrao = "01001000";

    public const string LogradouroAtualizado = "Rua B";
    public const string NumeroAtualizado = "200";
    public const string BairroAtualizado = "Bairro Novo";
    public const string CidadeAtualizada = "Santo Andre";
    public const string CepAtualizado = "09000000";

    public const string TelefonePadrao = "(11) 99999-9999";
    public const string TelefoneNormalizadoPadrao = "11999999999";
    public const string TelefoneAtualizado = "(11) 98888-7777";
    public const string TelefoneAtualizadoNormalizado = "11988887777";

    public const string EmailPadrao = "cliente@email.com";
    public const string EmailAtualizado = "novo@email.com";

    public static CpfCnpj CriarDocumentoPadrao()
    {
        return CpfCnpj.Criar(DocumentoPadrao);
    }

    public static CpfCnpj CriarCnpjPadrao()
    {
        return CpfCnpj.Criar(CnpjPadrao);
    }

    public static Endereco CriarEnderecoPadrao()
    {
        return Endereco.Criar(LogradouroPadrao, NumeroPadrao, BairroPadrao, CidadePadrao, CepPadrao);
    }

    public static Endereco CriarEnderecoAtualizado()
    {
        return Endereco.Criar(LogradouroAtualizado, NumeroAtualizado, BairroAtualizado, CidadeAtualizada, CepAtualizado);
    }

    public static Telefone CriarTelefonePadrao()
    {
        return Telefone.Criar(TelefonePadrao);
    }

    public static Telefone CriarTelefoneAtualizado()
    {
        return Telefone.Criar(TelefoneAtualizado);
    }

    public static Email CriarEmailPadrao()
    {
        return Email.Criar(EmailPadrao);
    }

    public static Email CriarEmailAtualizado()
    {
        return Email.Criar(EmailAtualizado);
    }

    public static Cliente CriarClientePadrao()
    {
        return Cliente.Criar(CriarDocumentoPadrao(), NomePadrao, CriarEnderecoPadrao(), CriarTelefonePadrao(), CriarEmailPadrao());
    }
}
