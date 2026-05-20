using OficinaMecanica.API.Middlewares;

namespace OficinaMecanica.API.Extensions.Middlewares;

public static class ApiApplicationBuilderExtensions
{
    public static WebApplication UseApiMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

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
