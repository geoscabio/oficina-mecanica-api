namespace OficinaMecanica.Domain.Atendimento.Messages;

public static class ClienteErrorMessages
{
    public const string ClienteNaoEncontrado = "Cliente não encontrado.";
    public const string ClienteDuplicado = "Cliente já cadastrado para o documento informado.";
    public const string DocumentoObrigatorio = "Documento do cliente é obrigatório.";
    public const string NomeObrigatorio = "Nome do cliente é obrigatório.";
    public const string EnderecoObrigatorio = "Endereço do cliente é obrigatório.";
    public const string TelefoneObrigatorio = "Telefone do cliente é obrigatório.";
    public const string EmailObrigatorio = "E-mail do cliente é obrigatório.";
    public const string DocumentoInvalido = "CPF/CNPJ inválido.";
    public const string EmailInvalido = "E-mail inválido.";
    public const string EnderecoInvalido = "Endereço inválido.";
    public const string CepInvalido = "CEP inválido.";
    public const string TelefoneInvalido = "Telefone inválido.";
}
