using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;

public sealed class CadastrarServicoCatalogoUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IValidator<CadastrarServicoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public CadastrarServicoCatalogoUseCase(IServicoCatalogoRepository servicoCatalogoRepository, IValidator<CadastrarServicoCatalogoRequest> validator, IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ServicoCatalogoResponse>> ExecuteAsync(CadastrarServicoCatalogoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ServicoCatalogoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var servicoCatalogo = ServicoCatalogo.Criar(request.Descricao, request.Valor);

        await _servicoCatalogoRepository.AdicionarAsync(servicoCatalogo, cancellationToken);

        return Result<ServicoCatalogoResponse>.Ok(_mapper.Map<ServicoCatalogoResponse>(servicoCatalogo));
    }
}

