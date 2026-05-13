namespace OficinaMecanica.Application.GestaoOrdemServico.ValidationMessages;

public static class OrdemServicoValidationMessages
{
    public const string RequestAberturaOrdemServicoObrigatorio = "Request de abertura de ordem de serviço é obrigatório.";
    public const string RequestIniciarDiagnosticoObrigatorio = "Request para iniciar diagnóstico é obrigatório.";
    public const string RequestDefinirServicosObrigatorio = "Request para definir serviços é obrigatório.";
    public const string RequestReservarPecaInsumoObrigatorio = "Request para reservar peça ou insumo é obrigatório.";
    public const string RequestAguardarAprovacaoOrcamentoObrigatorio = "Request para aguardar aprovação do orçamento é obrigatório.";
    public const string RequestIniciarExecucaoOrdemServicoObrigatorio = "Request para iniciar execução da ordem de serviço é obrigatório.";
    public const string RequestIniciarExecucaoServicoObrigatorio = "Request para iniciar execução do serviço é obrigatório.";
    public const string RequestFinalizarServicoObrigatorio = "Request para finalizar serviço é obrigatório.";
    public const string RequestFinalizarOrdemServicoObrigatorio = "Request para finalizar ordem de serviço é obrigatório.";
    public const string RequestCancelarOrdemServicoObrigatorio = "Request para cancelar ordem de serviço é obrigatório.";
    public const string RequestEntregarOrdemServicoObrigatorio = "Request para entregar ordem de serviço é obrigatório.";
    public const string RequestDetalharOrdemServicoObrigatorio = "Request para detalhar ordem de serviço é obrigatório.";
    public const string RequestListarOrdensServicoObrigatorio = "Request para listar ordens de serviço é obrigatório.";
    public const string RequestConsultarStatusOrdemServicoObrigatorio = "Request para consultar status da ordem de serviço é obrigatório.";
    public const string VeiculoIdObrigatorio = "VeículoId é obrigatório.";
    public const string MecanicoIdObrigatorio = "MecânicoId é obrigatório.";
    public const string OrdemServicoIdObrigatorio = "OrdemServicoId é obrigatório.";
    public const string ServicoIdObrigatorio = "ServicoId é obrigatório.";
    public const string MotivoCancelamentoObrigatorio = "Motivo do cancelamento é obrigatório.";
    public const string PaginaMaiorQueZero = "Página deve ser maior que zero.";
    public const string TamanhoPaginaInvalido = "Tamanho da página deve estar entre 1 e 100.";
    public const string ServicosCatalogoIdsObrigatorio = "ServiçosCatalogoIds é obrigatório.";
    public const string ServicoCatalogoIdObrigatorio = "ServiçoCatalogoId é obrigatório.";
    public const string PecasInsumosObrigatorio = "PeçasInsumos é obrigatório.";
    public const string PecasInsumosSemItensRepetidos = "PeçasInsumos não pode possuir itens repetidos.";
    public const string PecaInsumoCatalogoIdObrigatorio = "PeçaInsumoCatalogoId é obrigatório.";
    public const string QuantidadeMaiorQueZero = "Quantidade deve ser maior que zero.";
}
