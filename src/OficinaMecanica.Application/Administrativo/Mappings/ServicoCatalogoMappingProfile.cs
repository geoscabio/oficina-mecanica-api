using AutoMapper;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.Administrativo.Mappings;

public sealed class ServicoCatalogoMappingProfile : Profile
{
    public const string TempoMedioExecucaoEmMinutosKey = "TempoMedioExecucaoEmMinutos";

    public ServicoCatalogoMappingProfile()
    {
        CreateMap<ServicoCatalogo, ServicoCatalogoResponse>();

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
