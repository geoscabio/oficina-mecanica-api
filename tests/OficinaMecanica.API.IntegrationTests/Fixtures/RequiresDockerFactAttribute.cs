using System.IO.Pipes;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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

        return DockerHostDisponivelPorVariavel()
            || DockerDisponivelNoSistemaOperacional();
    }

    private static bool DockerHostDisponivelPorVariavel()
    {
        var dockerHost = Environment.GetEnvironmentVariable("DOCKER_HOST");

        if (string.IsNullOrWhiteSpace(dockerHost)
            || !Uri.TryCreate(dockerHost, UriKind.Absolute, out var dockerHostUri))
        {
            return false;
        }

        return dockerHostUri.Scheme switch
        {
            "npipe" => ConsegueConectarPipe(dockerHostUri.Segments.LastOrDefault()?.TrimEnd('/') ?? "docker_engine"),
            "unix" => ConsegueConectarUnixSocket(dockerHostUri.LocalPath),
            "tcp" => ConsegueConectarTcp(dockerHostUri.Host, dockerHostUri.Port),
            _ => false
        };
    }

    private static bool DockerDisponivelNoSistemaOperacional()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return ConsegueConectarPipe("docker_engine")
                || ConsegueConectarPipe("dockerDesktopLinuxEngine");
        }

        return ConsegueConectarUnixSocket("/var/run/docker.sock");
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

    private static bool ConsegueConectarUnixSocket(string socketPath)
    {
        if (string.IsNullOrWhiteSpace(socketPath) || !File.Exists(socketPath))
        {
            return false;
        }

        try
        {
            using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

            socket.Connect(new UnixDomainSocketEndPoint(socketPath));
            return socket.Connected;
        }
        catch
        {
            return false;
        }
    }

    private static bool ConsegueConectarTcp(string host, int port)
    {
        if (string.IsNullOrWhiteSpace(host) || port <= 0)
        {
            return false;
        }

        try
        {
            using var client = new TcpClient();

            client.Connect(host, port);
            return client.Connected;
        }
        catch
        {
            return false;
        }
    }
}
