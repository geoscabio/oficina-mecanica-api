using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed record AbrirOrdemServicoRepositorios(
    IOrdemServicoRepository OrdemServico,
    IClienteRepository Cliente,
    IVeiculoRepository Veiculo,
    IMecanicoRepository Mecanico,
    IServicoCatalogoRepository ServicoCatalogo,
    IPecaInsumoCatalogoRepository PecaInsumoCatalogo,
    IEstoqueRepository Estoque);
