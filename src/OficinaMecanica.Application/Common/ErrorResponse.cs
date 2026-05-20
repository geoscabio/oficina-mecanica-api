namespace OficinaMecanica.Application.Common;

public enum TipoErro
{
    Validacao = 1,
    NaoEncontrado = 2,
    RegraNegocio = 3,
    NaoAutorizado = 4,
    ErroInterno = 5,
    AcessoProibido = 6
}

public sealed record ErrorResponse(string Mensagem, TipoErro Tipo);
