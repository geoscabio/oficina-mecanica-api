using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.CadastrarPecaInsumoCatalogo;

public sealed record CadastrarPecaInsumoCatalogoRequest(
    string Descricao,
    TipoPecaInsumo Tipo,
    decimal Valor);