namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.AtualizarServicoCatalogo;

public sealed record AtualizarServicoCatalogoRequest(Guid ServicoCatalogoId, string Descricao, decimal Valor);
