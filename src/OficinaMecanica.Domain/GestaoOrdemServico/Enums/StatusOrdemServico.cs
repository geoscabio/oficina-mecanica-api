namespace OficinaMecanica.Domain.GestaoOrdemServico.Enums;

public enum StatusOrdemServico
{
    RECEBIDA = 1,
    EM_DIAGNOSTICO = 2,
    AGUARDANDO_APROVACAO = 3,
    EM_EXECUCAO = 4,
    FINALIZADA = 5,
    CANCELADA = 6,
    ENTREGUE = 7
}
