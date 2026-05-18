namespace OficinaMecanica.API.Administrativo.Requests;

public sealed record AtualizarServicoCatalogoApiRequest(
    string Descricao,
    decimal Valor);
