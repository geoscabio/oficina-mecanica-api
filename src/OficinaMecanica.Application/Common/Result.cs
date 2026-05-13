namespace OficinaMecanica.Application.Common;

public sealed class Result<T>
{
    private Result(bool sucesso, T? valor, ErrorResponse? erro)
    {
        Sucesso = sucesso;
        Valor = valor;
        Erro = erro;
    }

    public bool Sucesso { get; }
    public T? Valor { get; }
    public ErrorResponse? Erro { get; }

    public static Result<T> Ok(T valor)
    {
        return new Result<T>(true, valor, null);
    }

    public static Result<T> Falha(string erro)
    {
        return new Result<T>(false, default, new ErrorResponse(erro));
    }
}
