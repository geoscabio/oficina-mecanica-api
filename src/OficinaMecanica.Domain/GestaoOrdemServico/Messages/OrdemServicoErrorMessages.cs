namespace OficinaMecanica.Domain.GestaoOrdemServico.Messages;

public static class OrdemServicoErrorMessages
{
    public const string OrdemServicoNaoEncontrada = "Ordem de servico nao encontrada.";
    public const string NumeroObrigatorio = "Numero da ordem de servico e obrigatorio.";
    public const string VeiculoObrigatorio = "Veiculo da ordem de servico e obrigatorio.";
    public const string MecanicoObrigatorio = "Mecanico da ordem de servico e obrigatorio.";
    public const string ServicoObrigatorioParaAguardarAprovacao = "Ordem de servico deve possuir ao menos um servico para aguardar aprovacao.";
    public const string ServicosFinalizadosObrigatorios = "Todos os servicos devem estar finalizados para finalizar a ordem de servico.";
    public const string CancelamentoStatusInvalido = "Ordem de servico nao pode ser cancelada no status atual.";
    public const string MotivoCancelamentoInvalido = "Motivo de cancelamento da ordem de servico invalido.";
    public const string DecisaoOrcamentoInvalida = "Decisao do orcamento invalida.";
    public const string ServicoNaoEncontrado = "Servico nao encontrado na ordem de servico.";
    public const string TransicaoStatusInvalida = "Transicao de status da ordem de servico invalida.";
    public const string PecaInsumoCatalogoObrigatorio = "Peca ou insumo do catalogo e obrigatorio.";
    public const string QuantidadePecaInsumoMaiorQueZero = "Quantidade da peca ou insumo deve ser maior que zero.";
    public const string ValorUnitarioPecaInsumoMaiorQueZero = "Valor unitario da peca ou insumo deve ser maior que zero.";
    public const string ServicoCatalogoObrigatorio = "Servico do catalogo e obrigatorio.";
    public const string ValorServicoMaiorQueZero = "Valor do servico deve ser maior que zero.";
    public const string ServicoPendenteParaIniciarExecucao = "Servico deve estar pendente para iniciar execucao.";
    public const string ServicoEmExecucaoParaFinalizar = "Servico deve estar em execucao para finalizar.";
}
