namespace OficinaMecanica.Infrastructure.Identidade.Options;

public sealed class IdentidadeOptions
{
    public const string SectionName = "Identidade";

    public IReadOnlyCollection<UsuarioDemoOptions> UsuariosDemo { get; init; } = [];
}

public sealed class UsuarioDemoOptions
{
    public string Nome { get; init; } = string.Empty;
    public string Login { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string Perfil { get; init; } = string.Empty;

    public bool EhValido()
    {
        return !string.IsNullOrWhiteSpace(Nome)
            && !string.IsNullOrWhiteSpace(Login)
            && !string.IsNullOrWhiteSpace(Senha)
            && !string.IsNullOrWhiteSpace(Perfil);
    }
}
