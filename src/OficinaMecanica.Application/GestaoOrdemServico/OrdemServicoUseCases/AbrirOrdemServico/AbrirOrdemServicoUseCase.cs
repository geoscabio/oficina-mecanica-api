using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed class AbrirOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<AbrirOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;

    public AbrirOrdemServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IClienteRepository clienteRepository,
        IVeiculoRepository veiculoRepository,
        IMecanicoRepository mecanicoRepository,
        IServicoCatalogoRepository servicoCatalogoRepository,
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IEstoqueRepository estoqueRepository,
        IUnitOfWork unitOfWork,
        IValidator<AbrirOrdemServicoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _clienteRepository = clienteRepository;
        _veiculoRepository = veiculoRepository;
        _mecanicoRepository = mecanicoRepository;
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _estoqueRepository = estoqueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(AbrirOrdemServicoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<OrdemServicoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var cliente = await ObterClienteAsync(request, cancellationToken);
        if (cliente is null)
        {
            return Result<OrdemServicoResponse>.Falha(ClienteErrorMessages.ClienteNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null || veiculo.ClienteId != cliente.Id)
        {
            return Result<OrdemServicoResponse>.Falha(VeiculoErrorMessages.VeiculoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var mecanico = await _mecanicoRepository.ObterPorIdAsync(request.MecanicoId, cancellationToken);
        if (mecanico is null)
        {
            return Result<OrdemServicoResponse>.Falha(MecanicoErrorMessages.MecanicoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var numero = await _ordemServicoRepository.ObterProximoNumeroAsync(cancellationToken);
        var ordemServico = OrdemServico.Abrir(numero, request.VeiculoId, request.MecanicoId);
        var servicosCatalogoIds = request.ServicosCatalogoIds ?? [];
        var pecasInsumos = request.PecasInsumos ?? [];

        var servicosCatalogo = await ObterServicosCatalogoAsync(servicosCatalogoIds, cancellationToken);
        if (servicosCatalogoIds.Any(servicoCatalogoId => !servicosCatalogo.ContainsKey(servicoCatalogoId)))
        {
            return Result<OrdemServicoResponse>.Falha(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        foreach (var servicoCatalogoId in servicosCatalogoIds)
        {
            var servicoCatalogo = servicosCatalogo[servicoCatalogoId];

            ordemServico.RegistrarServicoNaAbertura(servicoCatalogo.Id, servicoCatalogo.Valor);
        }

        Estoque? estoque = null;

        if (pecasInsumos.Count > 0)
        {
            estoque = await _estoqueRepository.ObterAsync(cancellationToken);
            if (estoque is null)
            {
                return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueNaoEncontrado, TipoErro.NaoEncontrado);
            }

            var pecasInsumosCatalogo = await ObterPecasInsumosCatalogoAsync(pecasInsumos, cancellationToken);
            if (pecasInsumos.Any(pecaInsumo => !pecasInsumosCatalogo.ContainsKey(pecaInsumo.PecaInsumoCatalogoId)))
            {
                return Result<OrdemServicoResponse>.Falha(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
            }

            if (!ExisteEstoqueDisponivel(estoque, pecasInsumos))
            {
                return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueInsuficiente, TipoErro.RegraNegocio);
            }

            foreach (var pecaInsumo in pecasInsumos)
            {
                var pecaInsumoCatalogo = pecasInsumosCatalogo[pecaInsumo.PecaInsumoCatalogoId];

                ordemServico.RegistrarPecaInsumoNaAbertura(pecaInsumoCatalogo.Id, pecaInsumo.Quantidade, pecaInsumoCatalogo.Valor);
                estoque.ReservarItens(pecaInsumoCatalogo.Id, pecaInsumo.Quantidade);
            }
        }

        if (estoque is null)
        {
            await _ordemServicoRepository.AdicionarAsync(ordemServico, cancellationToken);
        }
        else
        {
            await _unitOfWork.ExecutarEmTransacaoAsync(
                async transactionCancellationToken =>
                {
                    await _ordemServicoRepository.AdicionarAsync(ordemServico, transactionCancellationToken);
                    await _estoqueRepository.AtualizarAsync(estoque, transactionCancellationToken);
                },
                cancellationToken);
        }

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

    private async Task<Cliente?> ObterClienteAsync(AbrirOrdemServicoRequest request, CancellationToken cancellationToken)
    {
        if (request.ClienteId is Guid clienteId && clienteId != Guid.Empty)
        {
            return await _clienteRepository.ObterPorIdAsync(clienteId, cancellationToken);
        }

        var documento = CpfCnpj.Criar(request.DocumentoCliente!);

        return await _clienteRepository.ObterPorDocumentoAsync(documento.Numero, cancellationToken);
    }

    private async Task<Dictionary<Guid, ServicoCatalogo>> ObterServicosCatalogoAsync(IReadOnlyCollection<Guid> servicosCatalogoIds, CancellationToken cancellationToken)
    {
        if (servicosCatalogoIds.Count == 0)
        {
            return [];
        }

        var ids = servicosCatalogoIds
            .Distinct()
            .ToArray();

        var servicosCatalogo = await _servicoCatalogoRepository.ObterPorIdsAsync(ids, cancellationToken);

        return servicosCatalogo.ToDictionary(servicoCatalogo => servicoCatalogo.Id);
    }

    private async Task<Dictionary<Guid, PecaInsumoCatalogo>> ObterPecasInsumosCatalogoAsync(IEnumerable<PecaInsumoRequest> pecasInsumos, CancellationToken cancellationToken)
    {
        var ids = pecasInsumos
            .Select(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId)
            .Distinct()
            .ToArray();

        var pecasInsumosCatalogo = await _pecaInsumoCatalogoRepository.ObterPorIdsAsync(ids, cancellationToken);

        return pecasInsumosCatalogo.ToDictionary(pecaInsumoCatalogo => pecaInsumoCatalogo.Id);
    }

    private static bool ExisteEstoqueDisponivel(Estoque estoque, IEnumerable<PecaInsumoRequest> pecasInsumos)
    {
        return pecasInsumos.All(pecaInsumo => estoque.VerificarDisponibilidade(pecaInsumo.PecaInsumoCatalogoId, pecaInsumo.Quantidade));
    }
}
