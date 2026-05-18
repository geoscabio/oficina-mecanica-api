using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OficinaMecanica.Application;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Infrastructure;
using OficinaMecanica.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = _ =>
        new BadRequestObjectResult(new ErrorResponse(ValidationErrorMessages.RequestInvalido, TipoErro.Validacao));
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(builder.Configuration);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        var isDomainException = exception is DomainException;

        context.Response.StatusCode = isDomainException
            ? StatusCodes.Status409Conflict
            : StatusCodes.Status500InternalServerError;

        var error = isDomainException
            ? new ErrorResponse(exception!.Message, TipoErro.RegraNegocio)
            : new ErrorResponse("Erro interno inesperado.", TipoErro.RegraNegocio);

        await context.Response.WriteAsJsonAsync(error);
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
