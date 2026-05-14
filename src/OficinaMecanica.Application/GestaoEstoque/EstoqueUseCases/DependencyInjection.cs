using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddEstoqueUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<RegistrarEntradaEstoqueRequest>, RegistrarEntradaEstoqueValidator>();
        services.AddScoped<RegistrarEntradaEstoqueUseCase>();

        services.AddScoped<IValidator<ListarItensEstoqueRequest>, ListarItensEstoqueValidator>();
        services.AddScoped<ListarItensEstoqueUseCase>();

        return services;
    }
}