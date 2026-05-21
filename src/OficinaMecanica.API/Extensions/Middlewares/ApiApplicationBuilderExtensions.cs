using OficinaMecanica.API.Middlewares;

namespace OficinaMecanica.API.Extensions.Middlewares;

public static class ApiApplicationBuilderExtensions
{
    private const string ContentSecurityPolicy = "default-src 'self'; connect-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self' data:; object-src 'none'; base-uri 'self'; frame-ancestors 'none'; form-action 'self'";

    public static WebApplication UseApiMiddlewares(this WebApplication app)
    {
        app.UseSecurityHeaders();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Oficina Mecânica API v1");
            options.EnablePersistAuthorization();
        });

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
        app.MapControllers();
        app.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription();

        return app;
    }
}
