using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AguardarAprovacaoOrcamento;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DetalharOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.EntregarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases;

public static class DependencyInjection
{
    public static IServiceCollection AddOrdemServicoUseCases(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AbrirOrdemServicoRequest>, AbrirOrdemServicoValidator>();
        services.AddScoped<IValidator<IniciarDiagnosticoOrdemServicoRequest>, IniciarDiagnosticoOrdemServicoValidator>();
        services.AddScoped<IValidator<DefinirServicosRequest>, DefinirServicosValidator>();
        services.AddScoped<IValidator<ReservarPecaInsumoRequest>, ReservarPecaInsumoValidator>();
        services.AddScoped<IValidator<AguardarAprovacaoOrcamentoRequest>, AguardarAprovacaoOrcamentoValidator>();
        services.AddScoped<IValidator<IniciarExecucaoOrdemServicoRequest>, IniciarExecucaoOrdemServicoValidator>();
        services.AddScoped<IValidator<IniciarExecucaoServicoRequest>, IniciarExecucaoServicoValidator>();
        services.AddScoped<IValidator<FinalizarServicoRequest>, FinalizarServicoValidator>();
        services.AddScoped<IValidator<FinalizarOrdemServicoRequest>, FinalizarOrdemServicoValidator>();
        services.AddScoped<IValidator<CancelarOrdemServicoRequest>, CancelarOrdemServicoValidator>();
        services.AddScoped<IValidator<EntregarOrdemServicoRequest>, EntregarOrdemServicoValidator>();
        services.AddScoped<IValidator<DetalharOrdemServicoRequest>, DetalharOrdemServicoValidator>();

        services.AddScoped<AbrirOrdemServicoUseCase>();
        services.AddScoped<IniciarDiagnosticoOrdemServicoUseCase>();
        services.AddScoped<DefinirServicosUseCase>();
        services.AddScoped<ReservarPecaInsumoUseCase>();
        services.AddScoped<AguardarAprovacaoOrcamentoUseCase>();
        services.AddScoped<IniciarExecucaoOrdemServicoUseCase>();
        services.AddScoped<IniciarExecucaoServicoUseCase>();
        services.AddScoped<FinalizarServicoUseCase>();
        services.AddScoped<FinalizarOrdemServicoUseCase>();
        services.AddScoped<CancelarOrdemServicoUseCase>();
        services.AddScoped<EntregarOrdemServicoUseCase>();
        services.AddScoped<DetalharOrdemServicoUseCase>();

        return services;
    }
}
