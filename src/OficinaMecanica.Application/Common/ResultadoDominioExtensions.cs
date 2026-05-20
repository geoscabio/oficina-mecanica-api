using OficinaMecanica.Domain.Shared.Results;

namespace OficinaMecanica.Application.Common;

public static class ResultadoDominioExtensions
{
    public static Result<T> ParaFalhaDeRegraNegocio<T>(this ResultadoDominio resultado)
    {
        return Result<T>.Falha(resultado.Mensagem!, TipoErro.RegraNegocio);
    }

    public static Result<TResponse> ParaFalhaDeRegraNegocio<TDominio, TResponse>(
        this ResultadoDominio<TDominio> resultado)
    {
        return Result<TResponse>.Falha(resultado.Mensagem!, TipoErro.RegraNegocio);
    }
}
