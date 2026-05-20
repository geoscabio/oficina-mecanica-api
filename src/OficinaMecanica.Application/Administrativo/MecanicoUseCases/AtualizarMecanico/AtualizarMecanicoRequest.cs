namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;

public sealed record AtualizarMecanicoRequest(Guid MecanicoId, string Nome, string Funcional);