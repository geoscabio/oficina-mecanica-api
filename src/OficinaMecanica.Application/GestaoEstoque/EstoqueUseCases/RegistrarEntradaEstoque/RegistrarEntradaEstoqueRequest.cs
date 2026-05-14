namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

public sealed record RegistrarEntradaEstoqueRequest(
    Guid PecaInsumoCatalogoId,
    int Quantidade);