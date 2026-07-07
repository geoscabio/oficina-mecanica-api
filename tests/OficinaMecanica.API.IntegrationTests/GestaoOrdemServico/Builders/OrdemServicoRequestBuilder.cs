using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.NotificarDecisaoOrcamento;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;

namespace OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Builders;

public sealed class OrdemServicoRequestBuilder
{
    private Guid? _clienteId;
    private string? _documentoCliente;
    private Guid _veiculoId = Guid.NewGuid();
    private Guid _mecanicoId = Guid.NewGuid();
    private IReadOnlyCollection<Guid> _servicosCatalogoIds = [];
    private IReadOnlyCollection<PecaInsumoRequest> _pecasInsumos = [];
    private MotivoCancelamentoOrdemServico _motivo = MotivoCancelamentoOrdemServico.ReprovacaoOrcamento;

    public static OrdemServicoRequestBuilder Novo()
    {
        return new OrdemServicoRequestBuilder();
    }

    public OrdemServicoRequestBuilder ComClienteId(Guid clienteId)
    {
        _clienteId = clienteId;
        _documentoCliente = null;

        return this;
    }

    public OrdemServicoRequestBuilder ComDocumentoCliente(string documentoCliente)
    {
        _documentoCliente = documentoCliente;
        _clienteId = null;

        return this;
    }

    public OrdemServicoRequestBuilder ComVeiculoId(Guid veiculoId)
    {
        _veiculoId = veiculoId;

        return this;
    }

    public OrdemServicoRequestBuilder ComMecanicoId(Guid mecanicoId)
    {
        _mecanicoId = mecanicoId;

        return this;
    }

    public OrdemServicoRequestBuilder ComServicoCatalogoId(Guid servicoCatalogoId)
    {
        _servicosCatalogoIds = [servicoCatalogoId];

        return this;
    }

    public OrdemServicoRequestBuilder ComPecaInsumo(Guid pecaInsumoCatalogoId, int quantidade)
    {
        _pecasInsumos = [new PecaInsumoRequest(pecaInsumoCatalogoId, quantidade)];

        return this;
    }

    public AbrirOrdemServicoRequest BuildAbertura()
    {
        return new AbrirOrdemServicoRequest(_clienteId, _documentoCliente, _veiculoId, _mecanicoId, _servicosCatalogoIds, _pecasInsumos);
    }

    public DefinirServicosRequest BuildDefinicaoServicos(Guid ordemServicoId)
    {
        return new DefinirServicosRequest(ordemServicoId, _servicosCatalogoIds);
    }

    public ReservarPecaInsumoRequest BuildReservaPecasInsumos(Guid ordemServicoId)
    {
        return new ReservarPecaInsumoRequest(ordemServicoId, _pecasInsumos);
    }

    public CancelarOrdemServicoRequest BuildCancelamento(Guid ordemServicoId)
    {
        return new CancelarOrdemServicoRequest(ordemServicoId, _motivo);
    }

    public NotificarDecisaoOrcamentoRequest BuildNotificacaoOrcamento(Guid ordemServicoId, DecisaoOrcamento decisao)
    {
        return new NotificarDecisaoOrcamentoRequest(ordemServicoId, decisao);
    }
}
