namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;

public sealed record MecanicoResponse(
    Guid Id,
    string Nome,
    string Funcional);