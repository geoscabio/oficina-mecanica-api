using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;

public sealed record AtualizarPecaInsumoCatalogoRequest(
    Guid PecaInsumoCatalogoId,
    string Descricao,
    TipoPecaInsumo Tipo,
    decimal Valor);