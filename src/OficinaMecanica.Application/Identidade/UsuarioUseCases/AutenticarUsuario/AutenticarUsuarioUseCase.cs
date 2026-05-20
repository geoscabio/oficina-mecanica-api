using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.Identidade.Interfaces;
using OficinaMecanica.Application.Identidade.ValidationMessages;

namespace OficinaMecanica.Application.Identidade.UsuarioUseCases.AutenticarUsuario;

public sealed class AutenticarUsuarioUseCase
{
    private readonly IUsuarioAutenticadoService _usuarioAutenticadoService;
    private readonly ITokenService _tokenService;
    private readonly IValidator<AutenticarUsuarioRequest> _validator;

    public AutenticarUsuarioUseCase(
        IUsuarioAutenticadoService usuarioAutenticadoService,
        ITokenService tokenService,
        IValidator<AutenticarUsuarioRequest> validator)
    {
        _usuarioAutenticadoService = usuarioAutenticadoService;
        _tokenService = tokenService;
        _validator = validator;
    }

    public async Task<Result<AutenticarUsuarioResponse>> ExecuteAsync(
        AutenticarUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<AutenticarUsuarioResponse>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var usuario = await _usuarioAutenticadoService.AutenticarAsync(
            request.Login,
            request.Senha,
            cancellationToken);

        if (usuario is null)
        {
            return Result<AutenticarUsuarioResponse>.Falha(
                IdentidadeValidationMessages.CredenciaisInvalidas,
                TipoErro.NaoAutorizado);
        }

        var token = _tokenService.GerarToken(
            usuario.UsuarioId,
            usuario.Nome,
            usuario.Login,
            usuario.Perfil);

        var response = usuario with
        {
            Token = token
        };

        return Result<AutenticarUsuarioResponse>.Ok(response);
    }
}
