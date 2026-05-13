using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Entities;

public sealed class Servico
{
    private Servico(Guid id, Guid servicoCatalogoId, decimal valor)
    {
        Id = id;
        ServicoCatalogoId = servicoCatalogoId;
        Valor = valor;
        Status = StatusServico.PENDENTE;
    }

    public Guid Id { get; private set; }
    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFim { get; private set; }
    public StatusServico Status { get; private set; }
    public Guid ServicoCatalogoId { get; private set; }
    public decimal Valor { get; private set; }

    public static Servico Criar(Guid servicoCatalogoId, decimal valor)
    {
        if (servicoCatalogoId == Guid.Empty)
        {
            throw new DomainException(OrdemServicoErrorMessages.ServicoCatalogoObrigatorio);
        }

        if (valor <= 0)
        {
            throw new DomainException(OrdemServicoErrorMessages.ValorServicoMaiorQueZero);
        }

        return new Servico(Guid.NewGuid(), servicoCatalogoId, valor);
    }

    public void IniciarExecucao()
    {
        if (Status != StatusServico.PENDENTE)
        {
            throw new DomainException(OrdemServicoErrorMessages.ServicoPendenteParaIniciarExecucao);
        }

        Status = StatusServico.EM_EXECUCAO;
        DataInicio = DateTime.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != StatusServico.EM_EXECUCAO)
        {
            throw new DomainException(OrdemServicoErrorMessages.ServicoEmExecucaoParaFinalizar);
        }

        Status = StatusServico.FINALIZADO;
        DataFim = DateTime.UtcNow;
    }
}

