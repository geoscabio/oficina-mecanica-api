namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.ListarMecanicos;

public sealed record ListarMecanicosRequest(int Pagina = 1, int TamanhoPagina = 10);