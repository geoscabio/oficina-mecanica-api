using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;

public sealed class AtualizarServicoCatalogoUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IValidator<AtualizarServicoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarServicoCatalogoUseCase(IServicoCatalogoRepository servicoCatalogoRepository, IValidator<AtualizarServicoCatalogoRequest> validator, IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ServicoCatalogoResponse>> ExecuteAsync(AtualizarServicoCatalogoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ServicoCatalogoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var servicoCatalogo = await _servicoCatalogoRepository.ObterPorIdAsync(request.ServicoCatalogoId, cancellationToken);

        if (servicoCatalogo is null)
        {
            return Result<ServicoCatalogoResponse>.Falha(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        servicoCatalogo.Atualizar(request.Descricao, request.Valor);

        await _servicoCatalogoRepository.AtualizarAsync(servicoCatalogo, cancellationToken);

        return Result<ServicoCatalogoResponse>.Ok(_mapper.Map<ServicoCatalogoResponse>(servicoCatalogo));
    }
}

