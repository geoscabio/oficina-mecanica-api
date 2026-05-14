namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ListarClientes;

public sealed record ListarClientesRequest(int Pagina = 1, int TamanhoPagina = 10);