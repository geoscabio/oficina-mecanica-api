using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;

public sealed class ListarClientesUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<ListarClientesRequest> _validator;
    private readonly IMapper _mapper;

    public ListarClientesUseCase(IClienteRepository clienteRepository, IValidator<ListarClientesRequest> validator, IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ClienteResponse>>> ExecuteAsync(ListarClientesRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<ClienteResponse>>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var clientes = await _clienteRepository.ListarAsync(request.Pagina, request.TamanhoPagina, cancellationToken);

        var totalItens = await _clienteRepository.ContarAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<ClienteResponse>>(clientes);

        var pagedResult = new PagedResult<ClienteResponse>(response, request.Pagina, request.TamanhoPagina, totalItens);

        return Result<PagedResult<ClienteResponse>>.Ok(pagedResult);
    }
}
