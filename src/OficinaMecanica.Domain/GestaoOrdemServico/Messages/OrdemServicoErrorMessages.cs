namespace OficinaMecanica.Domain.GestaoOrdemServico.Messages;

public static class OrdemServicoErrorMessages
{
    public const string OrdemServicoNaoEncontrada = "Ordem de serviço não encontrada.";
    public const string NumeroObrigatorio = "Número da ordem de serviço é obrigatório.";
    public const string VeiculoObrigatorio = "Veículo da ordem de serviço é obrigatório.";
    public const string MecanicoObrigatorio = "Mecânico da ordem de serviço é obrigatório.";
    public const string ServicoObrigatorioParaAguardarAprovacao = "Ordem de serviço deve possuir ao menos um serviço para aguardar aprovação.";
    public const string ServicosFinalizadosObrigatorios = "Todos os serviços devem estar finalizados para finalizar a ordem de serviço.";
    public const string CancelamentoStatusInvalido = "Ordem de serviço não pode ser cancelada no status atual.";
    public const string MotivoCancelamentoInvalido = "Motivo de cancelamento da ordem de serviço inválido.";
    public const string ServicoNaoEncontrado = "Serviço não encontrado na ordem de serviço.";
    public const string TransicaoStatusInvalida = "Transição de status da ordem de serviço inválida.";
    public const string PecaInsumoCatalogoObrigatorio = "Peça ou insumo do catálogo é obrigatório.";
    public const string QuantidadePecaInsumoMaiorQueZero = "Quantidade da peça ou insumo deve ser maior que zero.";
    public const string ValorUnitarioPecaInsumoMaiorQueZero = "Valor unitário da peça ou insumo deve ser maior que zero.";
    public const string ServicoCatalogoObrigatorio = "Serviço do catálogo é obrigatório.";
    public const string ValorServicoMaiorQueZero = "Valor do serviço deve ser maior que zero.";
    public const string ServicoPendenteParaIniciarExecucao = "Serviço deve estar pendente para iniciar execução.";
    public const string ServicoEmExecucaoParaFinalizar = "Serviço deve estar em execução para finalizar.";
}
