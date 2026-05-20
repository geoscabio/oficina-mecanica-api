namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ListarVeiculos;

public sealed record ListarVeiculosRequest(int Pagina = 1, int TamanhoPagina = 10);