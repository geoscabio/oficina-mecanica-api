namespace OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

public static class EstoqueValidationMessages
{
    public const string RequestRegistrarEntradaEstoqueObrigatorio = "Request para registrar entrada de estoque é obrigatório.";
    public const string PecaInsumoCatalogoObrigatorio = "Peça ou insumo do catálogo é obrigatório.";
    public const string QuantidadeMaiorQueZero = "Quantidade deve ser maior que zero.";
}