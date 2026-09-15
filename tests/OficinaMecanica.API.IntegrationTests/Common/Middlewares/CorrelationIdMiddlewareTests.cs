using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OficinaMecanica.API.Middlewares;

namespace OficinaMecanica.API.IntegrationTests.Common.Middlewares;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task Dado_CorrelationIdValido_Quando_ProcessarRequisicao_Entao_DevePropagarNoContextoEResposta()
    {
        // Arrange
        const string correlationId = "correlation-id-externo";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;
        var middleware = CriarMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Items[CorrelationIdMiddleware.ItemName].Should().Be(correlationId);
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString().Should().Be(correlationId);
    }

    [Fact]
    public async Task Dado_CorrelationIdAusente_Quando_ProcessarRequisicao_Entao_DeveGerarIdentificador()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var middleware = CriarMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        var correlationId = context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();
        correlationId.Should().HaveLength(32);
        context.Items[CorrelationIdMiddleware.ItemName].Should().Be(correlationId);
    }

    [Fact]
    public async Task Dado_CorrelationIdInvalido_Quando_ProcessarRequisicao_Entao_NaoDevePropagarValorRecebido()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = new string('a', 129);
        var middleware = CriarMiddleware();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString().Should().NotBe(new string('a', 129));
    }

    [Fact]
    public async Task Dado_CorrelationId_Quando_ProcessarRequisicao_Entao_DeveAdicionarAoEscopoDeLogging()
    {
        // Arrange
        const string correlationId = "correlation-id-log";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = correlationId;
        var logger = new ScopeCapturingLogger<CorrelationIdMiddleware>();
        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask, logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        logger.Scope.Should().ContainKey("x_correlation_id").WhoseValue.Should().Be(correlationId);
    }

    private static CorrelationIdMiddleware CriarMiddleware()
    {
        return new CorrelationIdMiddleware(_ => Task.CompletedTask, NullLogger<CorrelationIdMiddleware>.Instance);
    }

    private sealed class ScopeCapturingLogger<T> : ILogger<T>
    {
        public Dictionary<string, object> Scope { get; private set; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            Scope = ((IEnumerable<KeyValuePair<string, object>>)state).ToDictionary();

            return EmptyScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class EmptyScope : IDisposable
        {
            public static readonly EmptyScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
