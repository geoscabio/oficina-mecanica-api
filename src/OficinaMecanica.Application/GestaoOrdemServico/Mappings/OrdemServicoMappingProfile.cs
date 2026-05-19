using AutoMapper;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Application.GestaoOrdemServico.Mappings;

public sealed class OrdemServicoMappingProfile : Profile
{
    public const string TempoMedioExecucaoEmMinutosKey = "TempoMedioExecucaoEmMinutos";

    public OrdemServicoMappingProfile()
    {
        CreateMap<OrdemServico, OrdemServicoResponse>()
            .ForCtorParam(nameof(OrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<Servico, ServicoOrdemServicoResponse>()
            .ForCtorParam(nameof(ServicoOrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<PecaInsumo, PecaInsumoOrdemServicoResponse>();

        CreateMap<Servico, ServicoStatusResponse>()
            .ForCtorParam(nameof(ServicoStatusResponse.ServicoId), opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam(nameof(ServicoStatusResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<OrdemServico, ConsultarStatusOrdemServicoResponse>()
            .ForCtorParam(nameof(ConsultarStatusOrdemServicoResponse.OrdemServicoId), opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam(nameof(ConsultarStatusOrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<ServicoCatalogo, TempoMedioExecucaoServicoResponse>()
            .ForCtorParam(
                nameof(TempoMedioExecucaoServicoResponse.ServicoCatalogoId),
                opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam(
                nameof(TempoMedioExecucaoServicoResponse.TempoMedioExecucaoEmMinutos),
                opcao => opcao.MapFrom((_, contexto) => ObterTempoMedio(contexto)));
    }

    private static double? ObterTempoMedio(ResolutionContext contexto)
    {
        return contexto.Items.TryGetValue(TempoMedioExecucaoEmMinutosKey, out var valor)
            ? valor as double?
            : null;
    }
}

