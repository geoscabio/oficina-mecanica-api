using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Common;

internal static class TestDataFactory
{
    public static Cliente CriarClientePadrao()
    {
        return Cliente.Criar(
            CpfCnpj.Criar("529.982.247-25"),
            "Maria Silva",
            new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001-000"),
            Telefone.Criar("(11) 99999-9999"),
            Email.Criar("maria@email.com"));
    }

    public static Veiculo CriarVeiculoPadrao(Guid? clienteId = null)
    {
        return Veiculo.Criar(
            clienteId ?? Guid.NewGuid(),
            Placa.Criar("ABC-1234"),
            "Toyota",
            "Corolla",
            2020);
    }

    public static Mecanico CriarMecanicoPadrao()
    {
        return Mecanico.Criar("Jose Santos", "MEC-001");
    }

    public static OrdemServico CriarOrdemServicoRecebida()
    {
        return OrdemServico.Abrir(Guid.NewGuid(), Guid.NewGuid());
    }

    public static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServicoRecebida();
        ordemServico.IniciarDiagnostico();

        return ordemServico;
    }

    public static ServicoCatalogo CriarServicoCatalogoPadrao(
        string descricao = "Troca de oleo",
        decimal valor = 150m)
    {
        return ServicoCatalogo.Criar(descricao, valor);
    }
}
