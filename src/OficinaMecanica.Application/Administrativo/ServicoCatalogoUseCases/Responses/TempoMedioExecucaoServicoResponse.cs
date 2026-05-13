namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;

public sealed record TempoMedioExecucaoServicoResponse(
    Guid ServicoCatalogoId,
    string Descricao,
    decimal Valor,
    double? TempoMedioExecucaoEmMinutos);
