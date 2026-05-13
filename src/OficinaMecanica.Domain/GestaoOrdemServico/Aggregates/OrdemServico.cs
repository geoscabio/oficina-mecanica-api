using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

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
    public MotivoCancelamentoOrdemServico? MotivoCancelamento { get; private set; }
    public Guid VeiculoId { get; private set; }
    public Guid MecanicoId { get; private set; }
    public IReadOnlyCollection<Servico> Servicos => _servicos.AsReadOnly();
    public IReadOnlyCollection<PecaInsumo> PecasInsumos => _pecasInsumos.AsReadOnly();

    public static OrdemServico Abrir(int numero, Guid veiculoId, Guid mecanicoId)
    {
        if (numero <= 0)
        {
            throw new DomainException(OrdemServicoErrorMessages.NumeroObrigatorio);
        }

        if (veiculoId == Guid.Empty)
        {
            throw new DomainException(OrdemServicoErrorMessages.VeiculoObrigatorio);
        }

        if (mecanicoId == Guid.Empty)
        {
            throw new DomainException(OrdemServicoErrorMessages.MecanicoObrigatorio);
        }

        return new OrdemServico(Guid.NewGuid(), numero, veiculoId, mecanicoId);
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
            throw new DomainException(OrdemServicoErrorMessages.ServicoObrigatorioParaAguardarAprovacao);
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
            throw new DomainException(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);
        }

        Status = StatusOrdemServico.FINALIZADA;
        DataFim = DateTime.UtcNow;
    }

    public void Entregar()
    {
        ExigirStatus(StatusOrdemServico.FINALIZADA);

        Status = StatusOrdemServico.ENTREGUE;
    }

    public void Cancelar(MotivoCancelamentoOrdemServico motivo)
    {
        if (!Enum.IsDefined(typeof(MotivoCancelamentoOrdemServico), motivo))
        {
            throw new DomainException(OrdemServicoErrorMessages.MotivoCancelamentoInvalido);
        }

        if (Status is StatusOrdemServico.FINALIZADA or StatusOrdemServico.ENTREGUE or StatusOrdemServico.CANCELADA)
        {
            throw new DomainException(OrdemServicoErrorMessages.CancelamentoStatusInvalido);
        }

        Status = StatusOrdemServico.CANCELADA;
        MotivoCancelamento = motivo;
        DataFim = DateTime.UtcNow;
    }

    private Servico ObterServico(Guid servicoId)
    {
        return _servicos.SingleOrDefault(servico => servico.Id == servicoId)
            ?? throw new DomainException(OrdemServicoErrorMessages.ServicoNaoEncontrado);
    }

    private void ExigirStatus(StatusOrdemServico statusEsperado)
    {
        if (Status != statusEsperado)
        {
            throw new DomainException(OrdemServicoErrorMessages.TransicaoStatusInvalida);
        }
    }
}

