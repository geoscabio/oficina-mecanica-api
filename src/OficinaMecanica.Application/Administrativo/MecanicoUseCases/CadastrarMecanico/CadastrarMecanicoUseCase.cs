using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.CadastrarMecanico;

public sealed class CadastrarMecanicoUseCase
{
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IValidator<CadastrarMecanicoRequest> _validator;
    private readonly IMapper _mapper;

    public CadastrarMecanicoUseCase(IMecanicoRepository mecanicoRepository, IValidator<CadastrarMecanicoRequest> validator, IMapper mapper)
    {
        _mecanicoRepository = mecanicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<MecanicoResponse>> ExecuteAsync(CadastrarMecanicoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<MecanicoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var mecanico = Mecanico.Criar(request.Nome, request.Funcional);

        await _mecanicoRepository.AdicionarAsync(mecanico, cancellationToken);

        return Result<MecanicoResponse>.Ok(_mapper.Map<MecanicoResponse>(mecanico));
    }
}
