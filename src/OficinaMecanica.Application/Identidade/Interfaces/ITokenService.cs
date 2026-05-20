namespace OficinaMecanica.Application.Identidade.Interfaces;

public interface ITokenService
{
    string GerarToken(Guid usuarioId, string nome, string login, string perfil);
}