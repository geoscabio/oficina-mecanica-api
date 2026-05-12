using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed class CadastrarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly CadastrarClienteValidator _validator;

    public CadastrarClienteUseCase(IClienteRepository clienteRepository, CadastrarClienteValidator validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public async Task<Result<CadastrarClienteResponse>> ExecuteAsync(
        CadastrarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.Validate(request);

        var documento = CpfCnpj.Criar(request.Documento);
        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documento.Numero, cancellationToken);

        if (clienteExistente is not null)
        {
            return Result<CadastrarClienteResponse>.Falha("Cliente ja cadastrado para o documento informado.");
        }

        var cliente = Cliente.Criar(
            documento,
            request.Nome,
            new Endereco(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.CEP),
            Telefone.Criar(request.Telefone),
            Email.Criar(request.Email));

        await _clienteRepository.AdicionarAsync(cliente, cancellationToken);

        return Result<CadastrarClienteResponse>.Ok(new CadastrarClienteResponse(
            cliente.Id,
            cliente.Documento.Numero,
            cliente.Nome,
            cliente.Email.Endereco));
    }
}
