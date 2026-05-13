namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.CadastrarServicoCatalogo;

public sealed record CadastrarServicoCatalogoRequest(
    string Descricao,
    decimal Valor);
