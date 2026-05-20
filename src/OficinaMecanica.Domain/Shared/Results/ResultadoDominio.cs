namespace OficinaMecanica.Domain.Shared.Results;

public sealed record ResultadoDominio(bool Sucesso, string? Mensagem)
{
    public static ResultadoDominio Ok()
    {
        return new ResultadoDominio(true, null);
    }

    public static ResultadoDominio Falha(string mensagem)
    {
        return new ResultadoDominio(false, mensagem);
    }
}

public sealed record ResultadoDominio<T>(bool Sucesso, T? Valor, string? Mensagem)
{
    public static ResultadoDominio<T> Ok(T valor)
    {
        return new ResultadoDominio<T>(true, valor, null);
    }

    public static ResultadoDominio<T> Falha(string mensagem)
    {
        return new ResultadoDominio<T>(false, default, mensagem);
    }
}
