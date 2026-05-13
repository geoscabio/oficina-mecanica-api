namespace OficinaMecanica.Domain.GestaoEstoque.Messages;

public static class EstoqueErrorMessages
{
    public const string EstoqueNaoEncontrado = "Estoque não encontrado.";
    public const string EstoqueSemItens = "Estoque deve possuir ao menos um item.";
    public const string EstoqueComItemNulo = "Estoque não pode possuir item nulo.";
    public const string ItemNaoEncontrado = "Item de estoque não encontrado.";
    public const string PecaInsumoCatalogoObrigatorio = "Peça ou insumo do catálogo é obrigatório.";
    public const string QuantidadeDisponivelNaoNegativa = "Quantidade disponível não pode ser negativa.";
    public const string EstoqueInsuficiente = "Estoque insuficiente para reservar peça ou insumo.";
    public const string QuantidadeMaiorQueZero = "Quantidade deve ser maior que zero.";
    public const string QuantidadeReservadaInsuficiente = "Quantidade reservada insuficiente.";
}
