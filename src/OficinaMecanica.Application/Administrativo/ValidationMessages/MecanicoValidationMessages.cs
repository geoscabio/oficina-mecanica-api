namespace OficinaMecanica.Application.Administrativo.ValidationMessages;

public static class MecanicoValidationMessages
{
    public const string RequestCadastrarMecanicoObrigatorio = "Request para cadastrar mecânico é obrigatório.";
    public const string NomeObrigatorio = "Nome do mecânico é obrigatório.";
    public const string FuncionalObrigatorio = "Funcional do mecânico é obrigatório.";
    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";
}