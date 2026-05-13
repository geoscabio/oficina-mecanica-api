namespace OficinaMecanica.Application.Common;

public enum TipoErro
{
    Validacao = 1,
    NaoEncontrado = 2,
    RegraNegocio = 3
}

public sealed record ErrorResponse(string Mensagem, TipoErro Tipo);
