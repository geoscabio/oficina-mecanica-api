using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Results;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

public sealed class OrdemServico
{
    private readonly List<Servico> _servicos = new();
    private readonly List<PecaInsumo> _pecasInsumos = new();

    private OrdemServico()
    {
    }

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

    public ResultadoDominio IniciarDiagnostico()
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.RECEBIDA);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        Status = StatusOrdemServico.EM_DIAGNOSTICO;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio DefinirServico(Guid servicoCatalogoId, decimal valor)
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_DIAGNOSTICO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        _servicos.Add(Servico.Criar(servicoCatalogoId, valor));
        CalcularOrcamento();

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio ReservarPecaInsumo(Guid pecaInsumoCatalogoId, int quantidade, decimal valorUnitario)
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_DIAGNOSTICO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        _pecasInsumos.Add(PecaInsumo.Criar(pecaInsumoCatalogoId, quantidade, valorUnitario));
        CalcularOrcamento();

        return ResultadoDominio.Ok();
    }

    public void CalcularOrcamento()
    {
        ValorTotal = _servicos.Sum(servico => servico.Valor)
            + _pecasInsumos.Sum(pecaInsumo => pecaInsumo.ValorTotal);
    }

    public ResultadoDominio AguardarAprovacao()
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_DIAGNOSTICO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        if (_servicos.Count == 0)
        {
            return ResultadoDominio.Falha(OrdemServicoErrorMessages.ServicoObrigatorioParaAguardarAprovacao);
        }

        CalcularOrcamento();
        Status = StatusOrdemServico.AGUARDANDO_APROVACAO;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio IniciarExecucao()
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.AGUARDANDO_APROVACAO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        Status = StatusOrdemServico.EM_EXECUCAO;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio IniciarExecucaoServico(Guid servicoId)
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_EXECUCAO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        var resultadoServico = ObterServico(servicoId);
        if (!resultadoServico.Sucesso)
        {
            return ResultadoDominio.Falha(resultadoServico.Mensagem!);
        }

        return resultadoServico.Valor!.IniciarExecucao();
    }

    public ResultadoDominio FinalizarServico(Guid servicoId)
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_EXECUCAO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        var resultadoServico = ObterServico(servicoId);
        if (!resultadoServico.Sucesso)
        {
            return ResultadoDominio.Falha(resultadoServico.Mensagem!);
        }

        return resultadoServico.Valor!.Finalizar();
    }

    public ResultadoDominio Finalizar()
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.EM_EXECUCAO);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        if (_servicos.Count == 0 || _servicos.Any(servico => servico.Status != StatusServico.FINALIZADO))
        {
            return ResultadoDominio.Falha(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);
        }

        Status = StatusOrdemServico.FINALIZADA;
        DataFim = DateTime.UtcNow;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio Entregar()
    {
        var resultadoStatus = ValidarStatus(StatusOrdemServico.FINALIZADA);
        if (!resultadoStatus.Sucesso)
        {
            return resultadoStatus;
        }

        Status = StatusOrdemServico.ENTREGUE;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio Cancelar(MotivoCancelamentoOrdemServico motivo)
    {
        if (!Enum.IsDefined(typeof(MotivoCancelamentoOrdemServico), motivo))
        {
            return ResultadoDominio.Falha(OrdemServicoErrorMessages.MotivoCancelamentoInvalido);
        }

        if (Status is StatusOrdemServico.FINALIZADA or StatusOrdemServico.ENTREGUE or StatusOrdemServico.CANCELADA)
        {
            return ResultadoDominio.Falha(OrdemServicoErrorMessages.CancelamentoStatusInvalido);
        }

        Status = StatusOrdemServico.CANCELADA;
        MotivoCancelamento = motivo;
        DataFim = DateTime.UtcNow;

        return ResultadoDominio.Ok();
    }

    private ResultadoDominio<Servico> ObterServico(Guid servicoId)
    {
        var servico = _servicos.SingleOrDefault(servico => servico.Id == servicoId);

        return servico is null
            ? ResultadoDominio<Servico>.Falha(OrdemServicoErrorMessages.ServicoNaoEncontrado)
            : ResultadoDominio<Servico>.Ok(servico);
    }

    private ResultadoDominio ValidarStatus(StatusOrdemServico statusEsperado)
    {
        if (Status != statusEsperado)
        {
            return ResultadoDominio.Falha(OrdemServicoErrorMessages.TransicaoStatusInvalida);
        }

        return ResultadoDominio.Ok();
    }
}

