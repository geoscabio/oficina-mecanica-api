using OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.RemoverVeiculo;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.Factories;

internal static class VeiculoTestDataFactory
{
    public const string PlacaPadrao = "ABC-1234";
    public const string PlacaNormalizadaPadrao = "ABC1234";
    public const string MarcaPadrao = "Toyota";
    public const string ModeloPadrao = "Corolla";
    public const int AnoPadrao = 2020;

    public const string PlacaAtualizada = "XYZ-9876";
    public const string PlacaAtualizadaNormalizada = "XYZ9876";
    public const string MarcaAtualizada = "Honda";
    public const string ModeloAtualizado = "Civic";
    public const int AnoAtualizado = 2022;

    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static Veiculo CriarVeiculoPadrao(Guid? clienteId = null)
    {
        return Veiculo.Criar(clienteId ?? Guid.NewGuid(), Placa.Criar(PlacaPadrao), MarcaPadrao, ModeloPadrao, AnoPadrao);
    }

    public static CadastrarVeiculoRequest CriarCadastrarVeiculoRequestValido(Guid? clienteId = null, string placa = PlacaPadrao, string marca = MarcaPadrao, string modelo = ModeloPadrao, int ano = AnoPadrao)
    {
        return new CadastrarVeiculoRequest(clienteId ?? Guid.NewGuid(), placa, marca, modelo, ano);
    }

    public static AtualizarVeiculoRequest CriarAtualizarVeiculoRequestValido(Guid? veiculoId = null, string placa = PlacaAtualizada, string marca = MarcaAtualizada, string modelo = ModeloAtualizado, int ano = AnoAtualizado)
    {
        return new AtualizarVeiculoRequest(veiculoId ?? Guid.NewGuid(), placa, marca, modelo, ano);
    }

    public static ConsultarVeiculoRequest CriarConsultarVeiculoRequestValido(Guid? veiculoId = null)
    {
        return new ConsultarVeiculoRequest(veiculoId ?? Guid.NewGuid());
    }

    public static ConsultarVeiculoPorPlacaRequest CriarConsultarVeiculoPorPlacaRequestValido(string placa = PlacaPadrao)
    {
        return new ConsultarVeiculoPorPlacaRequest(placa);
    }

    public static ListarVeiculosRequest CriarListarVeiculosRequestValido(int pagina = PaginaPadrao, int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarVeiculosRequest(pagina, tamanhoPagina);
    }

    public static RemoverVeiculoRequest CriarRemoverVeiculoRequestValido(Guid? veiculoId = null)
    {
        return new RemoverVeiculoRequest(veiculoId ?? Guid.NewGuid());
    }
}