namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;

public sealed record AtualizarEstoqueRequest(
    Guid PecaInsumoCatalogoId,
    int QuantidadeDisponivel);