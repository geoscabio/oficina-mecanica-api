namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;

public sealed record ListarItensEstoqueRequest(int Pagina = 1, int TamanhoPagina = 10);