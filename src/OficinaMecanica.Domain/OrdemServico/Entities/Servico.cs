using OficinaMecanica.Domain.OrdemServico.Enums;
using OficinaMecanica.Domain.OrdemServico.Exceptions;

namespace OficinaMecanica.Domain.OrdemServico.Entities;

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
            throw new ServicoInvalidoException("Servico do catalogo e obrigatorio.");
        }

        if (valor <= 0)
        {
            throw new ServicoInvalidoException("Valor do servico deve ser maior que zero.");
        }

        return new Servico(Guid.NewGuid(), servicoCatalogoId, valor);
    }

    public void IniciarExecucao()
    {
        if (Status != StatusServico.PENDENTE)
        {
            throw new ServicoInvalidoException("Servico deve estar pendente para iniciar execucao.");
        }

        Status = StatusServico.EM_EXECUCAO;
        DataInicio = DateTime.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != StatusServico.EM_EXECUCAO)
        {
            throw new ServicoInvalidoException("Servico deve estar em execucao para finalizar.");
        }

        Status = StatusServico.FINALIZADO;
        DataFim = DateTime.UtcNow;
    }
}
