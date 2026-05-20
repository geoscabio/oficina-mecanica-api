using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;

public sealed class ListarVeiculosUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<ListarVeiculosRequest> _validator;
    private readonly IMapper _mapper;

    public ListarVeiculosUseCase(IVeiculoRepository veiculoRepository, IValidator<ListarVeiculosRequest> validator, IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<VeiculoResponse>>> ExecuteAsync(ListarVeiculosRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<VeiculoResponse>>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var veiculos = await _veiculoRepository.ListarAsync(request.Pagina, request.TamanhoPagina, cancellationToken);

        var totalItens = await _veiculoRepository.ContarAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<VeiculoResponse>>(veiculos);

        var pagedResult = new PagedResult<VeiculoResponse>(response, request.Pagina, request.TamanhoPagina, totalItens);

        return Result<PagedResult<VeiculoResponse>>.Ok(pagedResult);
    }
}
