using AutoMapper;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.GestaoOrdemServico.Mappings;

public sealed class OrdemServicoMappingProfile : Profile
{
    public OrdemServicoMappingProfile()
    {
        CreateMap<OrdemServico, OrdemServicoResponse>()
            .ForCtorParam(nameof(OrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));
    }
}

