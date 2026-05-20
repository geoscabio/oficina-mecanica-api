using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;

public sealed class ListarMecanicosUseCase
{
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IValidator<ListarMecanicosRequest> _validator;
    private readonly IMapper _mapper;

    public ListarMecanicosUseCase(IMecanicoRepository mecanicoRepository, IValidator<ListarMecanicosRequest> validator, IMapper mapper)
    {
        _mecanicoRepository = mecanicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<MecanicoResponse>>> ExecuteAsync(ListarMecanicosRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<MecanicoResponse>>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var mecanicos = await _mecanicoRepository.ListarAsync(request.Pagina, request.TamanhoPagina, cancellationToken);

        var totalItens = await _mecanicoRepository.ContarAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<MecanicoResponse>>(mecanicos);

        var pagedResult = new PagedResult<MecanicoResponse>(response, request.Pagina, request.TamanhoPagina, totalItens);

        return Result<PagedResult<MecanicoResponse>>.Ok(pagedResult);
    }
}
