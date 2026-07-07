using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using MecanicoTestDataFactory = OficinaMecanica.Application.UnitTests.Administrativo.Factories.MecanicoTestDataFactory;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public class AbrirOrdemServicoUseCaseTests
{
    private const int ProximoNumeroOrdemServico = 123;

    [Fact]
    public async Task Dado_RequestValidoComClienteId_Quando_AbrirOrdemServico_Entao_DevePersistirOrdemServicoERetornarSucesso()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var clienteRepository = CriarClienteRepository(cliente);
        var veiculoRepository = CriarVeiculoRepository(veiculo);
        var mecanicoRepository = CriarMecanicoRepository(mecanico);
        var useCase = CriarUseCase(ordemServicoRepository, clienteRepository, veiculoRepository, mecanicoRepository);
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(veiculo.Id, mecanico.Id, cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Numero.Should().Be(ProximoNumeroOrdemServico);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusRecebida);
        resultado.Valor.ValorTotal.Should().Be(0);
        resultado.Valor.DataInicio.Should().NotBe(default);
        resultado.Valor.DataFim.Should().BeNull();
        resultado.Valor.VeiculoId.Should().Be(veiculo.Id);
        resultado.Valor.MecanicoId.Should().Be(mecanico.Id);
        resultado.Valor.Servicos.Should().BeEmpty();
        resultado.Valor.PecasInsumos.Should().BeEmpty();

        clienteRepository.Verify(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()), Times.Once);
        veiculoRepository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);
        mecanicoRepository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(
            repo => repo.AdicionarAsync(It.Is<OrdemServico>(ordemServico => ordemServico.VeiculoId == veiculo.Id && ordemServico.MecanicoId == mecanico.Id && ordemServico.Numero == ProximoNumeroOrdemServico), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_RequestValidoComDocumentoCliente_Quando_AbrirOrdemServico_Entao_DeveConsultarClientePorDocumentoNormalizado()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var clienteRepository = CriarClienteRepository(cliente);
        var useCase = CriarUseCase(ordemServicoRepository, clienteRepository, CriarVeiculoRepository(veiculo), CriarMecanicoRepository(mecanico));
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculo.Id,
            mecanico.Id,
            clienteId: null,
            documentoCliente: ClienteTestDataFactory.DocumentoPadrao);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        clienteRepository.Verify(repo => repo.ObterPorDocumentoAsync(ClienteTestDataFactory.DocumentoNormalizadoPadrao, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ServicosEPecasNoPayload_Quando_AbrirOrdemServico_Entao_DeveRegistrarItensCalcularOrcamentoEReservarEstoque()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(pecaInsumoCatalogo.Id);
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var estoqueRepository = CriarEstoqueRepository(estoque);
        var unitOfWork = CriarUnitOfWork();
        var useCase = CriarUseCase(
            ordemServicoRepository,
            CriarClienteRepository(cliente),
            CriarVeiculoRepository(veiculo),
            CriarMecanicoRepository(mecanico),
            CriarServicoCatalogoRepository(servicoCatalogo),
            CriarPecaInsumoCatalogoRepository(pecaInsumoCatalogo),
            estoqueRepository,
            unitOfWork);
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculo.Id,
            mecanico.Id,
            cliente.Id,
            servicosCatalogoIds: new[] { servicoCatalogo.Id },
            pecasInsumos: new[] { new PecaInsumoRequest(pecaInsumoCatalogo.Id, OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Status.Should().Be(OrdemServicoTestDataFactory.StatusRecebida);
        resultado.Valor.Servicos.Should().ContainSingle();
        resultado.Valor.PecasInsumos.Should().ContainSingle();
        resultado.Valor.ValorTotal.Should().Be(servicoCatalogo.Valor + (pecaInsumoCatalogo.Valor * OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao));
        estoque.ItensEstoque.Single().QuantidadeReservada.Should().Be(OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao);
        unitOfWork.Verify(repo => repo.ExecutarEmTransacaoAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var clienteRepository = CriarClienteRepository(null);
        var veiculoRepository = CriarVeiculoRepository(null);
        var useCase = CriarUseCase(ordemServicoRepository, clienteRepository, veiculoRepository, CriarMecanicoRepository(null));
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(clienteId: Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
        veiculoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var veiculoRepository = CriarVeiculoRepository(null);
        var mecanicoRepository = CriarMecanicoRepository(mecanico);
        var useCase = CriarUseCase(ordemServicoRepository, CriarClienteRepository(cliente), veiculoRepository, mecanicoRepository);
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(mecanicoId: mecanico.Id, clienteId: cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
        veiculoRepository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);
        mecanicoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_VeiculoDeOutroCliente_Quando_AbrirOrdemServico_Entao_DeveRetornarVeiculoNaoEncontrado()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var useCase = CriarUseCase(
            ordemServicoRepository,
            CriarClienteRepository(cliente),
            CriarVeiculoRepository(veiculo),
            CriarMecanicoRepository(MecanicoTestDataFactory.CriarMecanicoPadrao()));
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(veiculo.Id, clienteId: cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_AbrirOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var mecanicoRepository = CriarMecanicoRepository(null);
        var useCase = CriarUseCase(ordemServicoRepository, CriarClienteRepository(cliente), CriarVeiculoRepository(veiculo), mecanicoRepository);
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(veiculo.Id, clienteId: cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
        mecanicoRepository.Verify(repo => repo.ObterPorIdAsync(request.MecanicoId, It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_PecaSemEstoqueDisponivel_Quando_AbrirOrdemServico_Entao_DeveRetornarRegraNegocio()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao(cliente.Id);
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(pecaInsumoCatalogo.Id, quantidadeDisponivel: 1);
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var useCase = CriarUseCase(
            ordemServicoRepository,
            CriarClienteRepository(cliente),
            CriarVeiculoRepository(veiculo),
            CriarMecanicoRepository(mecanico),
            pecaInsumoCatalogoRepository: CriarPecaInsumoCatalogoRepository(pecaInsumoCatalogo),
            estoqueRepository: CriarEstoqueRepository(estoque));
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(
            veiculo.Id,
            mecanico.Id,
            cliente.Id,
            pecasInsumos: new[] { new PecaInsumoRequest(pecaInsumoCatalogo.Id, 2) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueInsuficiente);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ClienteNaoInformado_Quando_AbrirOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, CriarClienteRepository(null), veiculoRepository, CriarMecanicoRepository(null));
        var request = new AbrirOrdemServicoRequest(null, null, Guid.NewGuid(), Guid.NewGuid(), [], []);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
        veiculoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_VeiculoIdVazio_Quando_AbrirOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var ordemServicoRepository = CriarOrdemServicoRepository();
        var veiculoRepository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, CriarClienteRepository(cliente), veiculoRepository, CriarMecanicoRepository(null));
        var request = OrdemServicoTestDataFactory.CriarAbrirOrdemServicoRequestValido(veiculoId: Guid.Empty, clienteId: cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
        veiculoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AdicionarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository()
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterProximoNumeroAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ProximoNumeroOrdemServico);

        return repository;
    }

    private static Mock<IClienteRepository> CriarClienteRepository(Cliente? cliente)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        repository
            .Setup(repo => repo.ObterPorDocumentoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        return repository;
    }

    private static Mock<IVeiculoRepository> CriarVeiculoRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static Mock<IMecanicoRepository> CriarMecanicoRepository(Mecanico? mecanico)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        return repository;
    }

    private static Mock<IServicoCatalogoRepository> CriarServicoCatalogoRepository(params ServicoCatalogo[] servicosCatalogo)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) => servicosCatalogo.Where(servicoCatalogo => ids.Contains(servicoCatalogo.Id)).ToArray());

        return repository;
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarPecaInsumoCatalogoRepository(params PecaInsumoCatalogo[] pecasInsumosCatalogo)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) => pecasInsumosCatalogo.Where(pecaInsumoCatalogo => ids.Contains(pecaInsumoCatalogo.Id)).ToArray());

        return repository;
    }

    private static Mock<IEstoqueRepository> CriarEstoqueRepository(Estoque? estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static Mock<IUnitOfWork> CriarUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork
            .Setup(repo => repo.ExecutarEmTransacaoAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns((Func<CancellationToken, Task> operacao, CancellationToken cancellationToken) => operacao(cancellationToken));

        return unitOfWork;
    }

    private static AbrirOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IClienteRepository> clienteRepository,
        Mock<IVeiculoRepository> veiculoRepository,
        Mock<IMecanicoRepository> mecanicoRepository,
        Mock<IServicoCatalogoRepository>? servicoCatalogoRepository = null,
        Mock<IPecaInsumoCatalogoRepository>? pecaInsumoCatalogoRepository = null,
        Mock<IEstoqueRepository>? estoqueRepository = null,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        return new AbrirOrdemServicoUseCase(
            ordemServicoRepository.Object,
            clienteRepository.Object,
            veiculoRepository.Object,
            mecanicoRepository.Object,
            (servicoCatalogoRepository ?? CriarServicoCatalogoRepository()).Object,
            (pecaInsumoCatalogoRepository ?? CriarPecaInsumoCatalogoRepository()).Object,
            (estoqueRepository ?? CriarEstoqueRepository(null)).Object,
            (unitOfWork ?? CriarUnitOfWork()).Object,
            new AbrirOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}
