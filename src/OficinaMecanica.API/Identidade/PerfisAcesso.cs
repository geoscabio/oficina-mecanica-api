namespace OficinaMecanica.API.Identidade;

public static class PerfisAcesso
{
    public const string Administrador = "Administrador";
    public const string Atendente = "Atendente";
    public const string Mecanico = "Mecanico";
    public const string Cliente = "Cliente";

    public const string AdministradorAtendente = Administrador + "," + Atendente;
    public const string AdministradorMecanico = Administrador + "," + Mecanico;
    public const string AdministradorAtendenteMecanico = Administrador + "," + Atendente + "," + Mecanico;
    public const string AdministradorAtendenteMecanicoCliente = Administrador + "," + Atendente + "," + Mecanico + "," + Cliente;
}
