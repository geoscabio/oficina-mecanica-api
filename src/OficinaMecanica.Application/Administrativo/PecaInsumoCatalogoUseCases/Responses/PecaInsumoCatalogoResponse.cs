using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;

public sealed record PecaInsumoCatalogoResponse(Guid Id, string Descricao, TipoPecaInsumo Tipo, decimal Valor);