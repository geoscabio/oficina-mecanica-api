namespace OficinaMecanica.Application.Administrativo.ValidationMessages;

public static class PecaInsumoCatalogoValidationMessages
{
    public const string RequestCadastrarPecaInsumoCatalogoObrigatorio = "Request para cadastrar peça ou insumo do catálogo é obrigatório.";
    public const string RequestAtualizarPecaInsumoCatalogoObrigatorio = "Request para atualizar peça ou insumo do catálogo é obrigatório.";
    public const string IdPecaInsumoCatalogoObrigatorio = "Id da peça ou insumo do catálogo é obrigatório.";
    public const string DescricaoObrigatoria = "Descrição da peça ou insumo é obrigatória.";
    public const string TipoInvalido = "Tipo da peça ou insumo é inválido.";
    public const string ValorMaiorQueZero = "Valor da peça ou insumo deve ser maior que zero.";
    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";
}