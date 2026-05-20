using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Entities;

public sealed class Servico
{
    private Servico()
    {
    }

    private Servico(Guid id, Guid servicoCatalogoId, decimal valor)
    {
        Id = id;
        ServicoCatalogoId = servicoCatalogoId;
        Valor = valor;
        Status = StatusServico.Pendente;
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
        if (Status != StatusServico.Pendente)
        {
            throw new DomainException(OrdemServicoErrorMessages.ServicoPendenteParaIniciarExecucao);
        }

        Status = StatusServico.EmExecucao;
        DataInicio = DateTime.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != StatusServico.EmExecucao)
        {
            throw new DomainException(OrdemServicoErrorMessages.ServicoEmExecucaoParaFinalizar);
        }

        Status = StatusServico.Finalizado;
        DataFim = DateTime.UtcNow;
    }
}


