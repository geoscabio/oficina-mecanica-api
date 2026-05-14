namespace OficinaMecanica.Application.GestaoEstoque.ValidationMessages;

public static class EstoqueValidationMessages
{
    public const string RequestRegistrarEntradaEstoqueObrigatorio = "Request para registrar entrada de estoque é obrigatório.";
    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";
    public const string PecaInsumoCatalogoObrigatorio = "Peça ou insumo do catálogo é obrigatório.";
    public const string QuantidadeMaiorQueZero = "Quantidade deve ser maior que zero.";
    public const string ItemEstoqueNaoEncontrado = "Item de estoque não encontrado.";
    public const string IdItemEstoqueObrigatorio = "Id do Item de estoque é obrigatório.";

}