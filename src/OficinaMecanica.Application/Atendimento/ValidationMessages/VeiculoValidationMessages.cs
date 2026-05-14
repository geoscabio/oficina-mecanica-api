namespace OficinaMecanica.Application.Atendimento.ValidationMessages;

public static class VeiculoValidationMessages
{
    public const string RequestCadastroVeiculoObrigatorio = "Request de cadastro de veículo é obrigatório.";
    public const string RequestAtualizarVeiculoObrigatorio = "Request de atualização de veículo é obrigatório.";

    public const string ClienteIdObrigatorio = "ClienteId é obrigatório.";
    public const string PlacaObrigatoria = "Placa é obrigatória.";
    public const string PlacaVeiculoObrigatoria = "Placa do veículo é obrigatória.";
    public const string IdVeiculoObrigatorio = "Id do veículo é obrigatório.";
    public const string MarcaObrigatoria = "Marca é obrigatória.";
    public const string ModeloObrigatorio = "Modelo é obrigatório.";
    public const string AnoMaiorQueZero = "Ano deve ser maior que zero.";

    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";

}
