using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

public sealed class OrdemServico
{
    private readonly List<Servico> _servicos = new();
    private readonly List<PecaInsumo> _pecasInsumos = new();

    private OrdemServico(Guid id, int numero, Guid veiculoId, Guid mecanicoId)
    {
        Id = id;
        Numero = numero;
        VeiculoId = veiculoId;
        MecanicoId = mecanicoId;
        Status = StatusOrdemServico.RECEBIDA;
        DataInicio = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusOrdemServico Status { get; private set; }
    public decimal ValorTotal { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public Guid VeiculoId { get; private set; }
    public Guid MecanicoId { get; private set; }
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<PecaInsumo> PecasInsumos => _pecasInsumos.AsReadOnly();

    public static OrdemServico Abrir(Guid veiculoId, Guid mecanicoId)
    {
        if (veiculoId == Guid.Empty)
        {
            throw new OrdemServicoInvalidaException("Veiculo da ordem de servico e obrigatorio.");
        }

        if (mecanicoId == Guid.Empty)
        {
            throw new OrdemServicoInvalidaException("Mecanico da ordem de servico e obrigatorio.");
        }

        return new OrdemServico(Guid.NewGuid(), Random.Shared.Next(1, int.MaxValue), veiculoId, mecanicoId);
    }

    public void IniciarDiagnostico()
    {
        ExigirStatus(StatusOrdemServico.RECEBIDA);

        Status = StatusOrdemServico.EM_DIAGNOSTICO;
    }

    public void DefinirServico(Guid servicoCatalogoId, decimal valor)
    {
        ExigirStatus(StatusOrdemServico.EM_DIAGNOSTICO);

        _servicos.Add(Servico.Criar(servicoCatalogoId, valor));
        CalcularOrcamento();
    }

    public void ReservarPecaInsumo(Guid pecaInsumoCatalogoId, int quantidade, decimal valorUnitario)
    {
        ExigirStatus(StatusOrdemServico.EM_DIAGNOSTICO);

        _pecasInsumos.Add(PecaInsumo.Criar(pecaInsumoCatalogoId, quantidade, valorUnitario));
        CalcularOrcamento();
    }

    public void CalcularOrcamento()
    {
        ValorTotal = _servicos.Sum(servico => servico.Valor)
            + _pecasInsumos.Sum(pecaInsumo => pecaInsumo.ValorTotal);
    }

    public void AguardarAprovacao()
    {
        ExigirStatus(StatusOrdemServico.EM_DIAGNOSTICO);

        if (_servicos.Count == 0)
        {
            throw new OrdemServicoInvalidaException("Ordem de servico deve possuir ao menos um servico para aguardar aprovacao.");
        }

        CalcularOrcamento();
        Status = StatusOrdemServico.AGUARDANDO_APROVACAO;
    }

    public void IniciarExecucao()
    {
        ExigirStatus(StatusOrdemServico.AGUARDANDO_APROVACAO);

        Status = StatusOrdemServico.EM_EXECUCAO;
    }

    public void IniciarExecucaoServico(Guid servicoId)
    {
        ExigirStatus(StatusOrdemServico.EM_EXECUCAO);

        ObterServico(servicoId).IniciarExecucao();
    }

    public void FinalizarServico(Guid servicoId)
    {
        ExigirStatus(StatusOrdemServico.EM_EXECUCAO);

        ObterServico(servicoId).Finalizar();
    }

    public void Finalizar()
    {
        ExigirStatus(StatusOrdemServico.EM_EXECUCAO);

        if (_servicos.Count == 0 || _servicos.Any(servico => servico.Status != StatusServico.FINALIZADO))
        {
            throw new OrdemServicoInvalidaException("Todos os servicos devem estar finalizados para finalizar a ordem de servico.");
        }

        Status = StatusOrdemServico.FINALIZADA;
        DataFim = DateTime.UtcNow;
    }

    public void Entregar()
    {
        ExigirStatus(StatusOrdemServico.FINALIZADA);

        Status = StatusOrdemServico.ENTREGUE;
    }

    public void Cancelar()
    {
        if (Status is StatusOrdemServico.FINALIZADA or StatusOrdemServico.ENTREGUE or StatusOrdemServico.CANCELADA)
        {
            throw new TransicaoStatusOrdemServicoInvalidaException("Ordem de servico nao pode ser cancelada no status atual.");
        }

        Status = StatusOrdemServico.CANCELADA;
        DataFim = DateTime.UtcNow;
    }

    private Servico ObterServico(Guid servicoId)
    {
        return _servicos.SingleOrDefault(servico => servico.Id == servicoId)
            ?? throw new ServicoInvalidoException("Servico nao encontrado na ordem de servico.");
    }

    private void ExigirStatus(StatusOrdemServico statusEsperado)
    {
        if (Status != statusEsperado)
        {
            throw new TransicaoStatusOrdemServicoInvalidaException("Transicao de status da ordem de servico invalida.");
        }
    }
}
