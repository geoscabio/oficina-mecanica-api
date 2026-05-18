using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.API.Administrativo.Requests;

public sealed record AtualizarPecaInsumoCatalogoApiRequest(
    string Descricao,
    TipoPecaInsumo Tipo,
    decimal Valor);
