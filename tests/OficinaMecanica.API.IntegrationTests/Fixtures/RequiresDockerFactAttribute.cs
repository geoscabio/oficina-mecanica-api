using System.IO.Pipes;

namespace OficinaMecanica.API.IntegrationTests.Fixtures;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RequiresDockerFactAttribute : FactAttribute
{
    private const string SkipMessage = "Docker nao esta acessivel neste ambiente. Teste de integracao foi ignorado.";
    private const string SkipDockerTestsEnvVar = "OFICINA_SKIP_DOCKER_TESTS";
    private static readonly Lazy<bool> DockerDisponivel = new(VerificarDisponibilidadeDocker);

    public RequiresDockerFactAttribute()
    {
        if (!DockerDisponivel.Value)
        {
            Skip = SkipMessage;
        }
    }

    private static bool VerificarDisponibilidadeDocker()
    {
        if (bool.TryParse(Environment.GetEnvironmentVariable(SkipDockerTestsEnvVar), out var skipByEnvironment)
            && skipByEnvironment)
        {
            return false;
        }

        return ConsegueConectarPipe("docker_engine")
            || ConsegueConectarPipe("dockerDesktopLinuxEngine");
    }

    private static bool ConsegueConectarPipe(string pipeName)
    {
        try
        {
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.None);

            client.Connect(250);
            return client.IsConnected;
        }
        catch
        {
            return false;
        }
    }
}
