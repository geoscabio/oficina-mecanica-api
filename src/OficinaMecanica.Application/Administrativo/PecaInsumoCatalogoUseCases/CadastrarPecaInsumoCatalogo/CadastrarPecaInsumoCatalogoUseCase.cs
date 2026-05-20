using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;

public sealed class CadastrarPecaInsumoCatalogoUseCase
{
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IValidator<CadastrarPecaInsumoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public CadastrarPecaInsumoCatalogoUseCase(IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository, IValidator<CadastrarPecaInsumoCatalogoRequest> validator, IMapper mapper)
    {
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PecaInsumoCatalogoResponse>> ExecuteAsync(CadastrarPecaInsumoCatalogoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var pecaInsumoCatalogo = PecaInsumoCatalogo.Criar(request.Descricao, request.Tipo, request.Valor);

        await _pecaInsumoCatalogoRepository.AdicionarAsync(pecaInsumoCatalogo, cancellationToken);

        return Result<PecaInsumoCatalogoResponse>.Ok(_mapper.Map<PecaInsumoCatalogoResponse>(pecaInsumoCatalogo));
    }
}
