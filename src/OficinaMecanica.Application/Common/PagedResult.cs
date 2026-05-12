namespace OficinaMecanica.Application.Common;

public sealed class PagedResult<T>
{
    public PagedResult(IReadOnlyCollection<T> itens, int pagina, int tamanhoPagina, int totalItens)
    {
        Itens = itens;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalItens = totalItens;
    }

    public IReadOnlyCollection<T> Itens { get; }
    public int Pagina { get; }
    public int TamanhoPagina { get; }
    public int TotalItens { get; }
}
