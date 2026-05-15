using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.Factories;

internal static class ClienteTestDataFactory
{
    public const string DocumentoPadrao = "529.982.247-25";
    public const string DocumentoNormalizadoPadrao = "52998224725";
    public const string NomePadrao = "Maria Silva";
    public const string TelefonePadrao = "(11) 99999-9999";
    public const string TelefoneNormalizadoPadrao = "11999999999";
    public const string EmailPadrao = "maria@email.com";

    public const string LogradouroPadrao = "Rua A";
    public const string NumeroPadrao = "100";
    public const string BairroPadrao = "Centro";
    public const string CidadePadrao = "Sao Paulo";
    public const string CepPadrao = "01001-000";
    public const string CepNormalizadoPadrao = "01001000";

    public const string NomeAtualizado = "Cliente Atualizado";
    public const string TelefoneAtualizado = "(11) 98888-7777";
    public const string TelefoneAtualizadoNormalizado = "11988887777";
    public const string EmailAtualizado = "novo@email.com";

    public const string LogradouroAtualizado = "Rua B";
    public const string NumeroAtualizado = "200";
    public const string BairroAtualizado = "Bairro Novo";
    public const string CidadeAtualizada = "Santo Andre";
    public const string CepAtualizado = "09000-000";

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static Cliente CriarClientePadrao()
    {
        return Cliente.Criar(
            CpfCnpj.Criar(DocumentoPadrao),
            NomePadrao,
            new Endereco(
                LogradouroPadrao,
                NumeroPadrao,
                BairroPadrao,
                CidadePadrao,
                CepPadrao),
            Telefone.Criar(TelefonePadrao),
            Email.Criar(EmailPadrao));
    }

    public static CadastrarClienteRequest CriarCadastrarClienteRequestValido(
        string documento = DocumentoPadrao,
        string nome = NomePadrao,
        string telefone = TelefonePadrao,
        string email = EmailPadrao)
    {
        return new CadastrarClienteRequest(
            documento,
            nome,
            CriarEnderecoRequestValido(),
            telefone,
            email);
    }

    public static AtualizarClienteRequest CriarAtualizarClienteRequestValido(
        Guid? clienteId = null,
        string nome = NomeAtualizado,
        string telefone = TelefoneAtualizado,
        string email = EmailAtualizado)
    {
        return new AtualizarClienteRequest(
            clienteId ?? Guid.NewGuid(),
            nome,
            CriarEnderecoAtualizadoRequestValido(),
            telefone,
            email);
    }

    public static ConsultarClienteRequest CriarConsultarClienteRequestValido(Guid? clienteId = null)
    {
        return new ConsultarClienteRequest(
            clienteId ?? Guid.NewGuid());
    }

    public static ConsultarClientePorDocumentoRequest CriarConsultarClientePorDocumentoRequestValido(
        string documento = DocumentoPadrao)
    {
        return new ConsultarClientePorDocumentoRequest(documento);
    }

    public static ListarClientesRequest CriarListarClientesRequestValido(
        int pagina = PaginaPadrao,
        int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarClientesRequest(
            pagina,
            tamanhoPagina);
    }

    public static RemoverClienteRequest CriarRemoverClienteRequestValido(
        Guid? clienteId = null)
    {
        return new RemoverClienteRequest(
            clienteId ?? Guid.NewGuid());
    }

    private static EnderecoRequest CriarEnderecoRequestValido()
    {
        return new EnderecoRequest(
            LogradouroPadrao,
            NumeroPadrao,
            BairroPadrao,
            CidadePadrao,
            CepPadrao);
    }

    private static EnderecoRequest CriarEnderecoAtualizadoRequestValido()
    {
        return new EnderecoRequest(
            LogradouroAtualizado,
            NumeroAtualizado,
            BairroAtualizado,
            CidadeAtualizada,
            CepAtualizado);
    }
}