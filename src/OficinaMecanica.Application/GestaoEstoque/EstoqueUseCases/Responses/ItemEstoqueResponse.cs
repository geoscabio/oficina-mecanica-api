namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;

public sealed record ItemEstoqueResponse(Guid Id, Guid PecaInsumoCatalogoId, int QuantidadeDisponivel, int QuantidadeReservada);