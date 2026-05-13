using AutoMapper;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;

namespace OficinaMecanica.Application.GestaoOrdemServico.Mappings;

public sealed class OrdemServicoMappingProfile : Profile
{
    public OrdemServicoMappingProfile()
    {
        CreateMap<OrdemServico, OrdemServicoResponse>()
            .ForCtorParam(nameof(OrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<Servico, ServicoStatusResponse>()
            .ForCtorParam(nameof(ServicoStatusResponse.ServicoId), opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam(nameof(ServicoStatusResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));

        CreateMap<OrdemServico, ConsultarStatusOrdemServicoResponse>()
            .ForCtorParam(nameof(ConsultarStatusOrdemServicoResponse.OrdemServicoId), opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam(nameof(ConsultarStatusOrdemServicoResponse.Status), opcao => opcao.MapFrom(origem => origem.Status.ToString()));
    }
}

