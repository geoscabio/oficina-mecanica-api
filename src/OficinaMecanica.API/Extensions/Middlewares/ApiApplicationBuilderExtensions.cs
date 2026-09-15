using OficinaMecanica.API.Middlewares;
using Microsoft.AspNetCore.Routing;
using Serilog;

namespace OficinaMecanica.API.Extensions.Middlewares;

public static class ApiApplicationBuilderExtensions
{
    private const string ContentSecurityPolicy = "default-src 'self'; connect-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self' data:; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'";

    public static WebApplication UseApiMiddlewares(this WebApplication app)
    {
        app.UseSecurityHeaders();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, context) =>
            {
                var endpoint = context.GetEndpoint();
                var route = (endpoint as RouteEndpoint)?.RoutePattern.RawText ?? "unmatched";

                diagnosticContext.Set("operation", endpoint?.DisplayName ?? "HTTP request");
                diagnosticContext.Set(
                    "http",
                    new
                    {
                        method = context.Request.Method,
                        route,
                        status_code = context.Response.StatusCode
                    },
                    destructureObjects: true);

                if (context.Items.TryGetValue(CorrelationIdMiddleware.ItemName, out var correlationId))
                {
                    diagnosticContext.Set("x_correlation_id", correlationId);
                }
            };
        });
        app.UseMiddleware<GlobalExceptionMiddleware>();

        if (IsSwaggerEnabled(app))
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Oficina Mecanica API v1");
                options.EnablePersistAuthorization();
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    private static WebApplication UseSecurityHeaders(this WebApplication app)
    {
        app.Use(
            async (context, next) =>
            {
                var headers = context.Response.Headers;

                headers.TryAdd("Content-Security-Policy", ContentSecurityPolicy);
                headers.TryAdd("Cross-Origin-Embedder-Policy", "require-corp");
                headers.TryAdd("Cross-Origin-Opener-Policy", "same-origin");
                headers.TryAdd("Cross-Origin-Resource-Policy", "same-origin");
                headers.TryAdd("Permissions-Policy", "camera=(), geolocation=(), microphone=()");
                headers.TryAdd("X-Content-Type-Options", "nosniff");
                headers.TryAdd("X-Frame-Options", "DENY");

                await next();
            });

        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/api/health")
            .AllowAnonymous()
            .WithName("HealthCheck")
            .WithTags("Health");

        app.MapControllers();
        app.MapGet("/", () => Results.Redirect(IsSwaggerEnabled(app) ? "/swagger" : "/api/health"))
            .ExcludeFromDescription();

        return app;
    }

    private static bool IsSwaggerEnabled(WebApplication app)
    {
        return app.Environment.IsDevelopment()
            || app.Environment.IsEnvironment("Testing")
            || (bool.TryParse(app.Configuration["Swagger:Enabled"], out var enabled) && enabled);
    }
}
