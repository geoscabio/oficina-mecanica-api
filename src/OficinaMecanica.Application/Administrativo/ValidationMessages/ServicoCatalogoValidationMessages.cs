namespace OficinaMecanica.Application.Administrativo.ValidationMessages;

public static class ServicoCatalogoValidationMessages
{
    public const string RequestCadastrarServicoCatalogoObrigatorio = "Request para cadastrar serviço do catálogo é obrigatório.";
    public const string RequestAtualizarServicoCatalogoObrigatorio = "Request para atualizar serviço do catálogo é obrigatório.";
    public const string RequestConsultarServicoCatalogoObrigatorio = "Request para consultar serviço do catálogo é obrigatório.";
    public const string RequestListarServicosCatalogoObrigatorio = "Request para listar serviços do catálogo é obrigatório.";
    public const string RequestRemoverServicoCatalogoObrigatorio = "Request para remover serviço do catálogo é obrigatório.";
    public const string RequestConsultarTempoMedioExecucaoServicoObrigatorio = "Request para consultar tempo médio de execução do serviço é obrigatório.";
    public const string RequestListarTempoMedioExecucaoServicosObrigatorio = "Request para listar tempos médios de execução dos serviços é obrigatório.";
    public const string ServicoCatalogoIdObrigatorio = "ServicoCatalogoId é obrigatório.";
    public const string DescricaoObrigatoria = "Descrição do serviço é obrigatória.";
    public const string ValorMaiorQueZero = "Valor do serviço deve ser maior que zero.";
    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";
}
