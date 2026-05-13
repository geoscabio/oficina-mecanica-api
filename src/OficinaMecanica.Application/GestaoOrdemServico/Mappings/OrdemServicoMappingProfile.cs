using AutoMapper;
using OficinaMecanica.Application.GestaoOrdemServico.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.GestaoOrdemServico.Mappings;

public sealed class OrdemServicoMappingProfile : Profile
{
    public OrdemServicoMappingProfile()
    {
        CreateMap<OrdemServico, OrdemServicoResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("Numero", opcao => opcao.MapFrom(origem => origem.Numero))
            .ForCtorParam("Status", opcao => opcao.MapFrom(origem => origem.Status.ToString()))
            .ForCtorParam("ValorTotal", opcao => opcao.MapFrom(origem => origem.ValorTotal))
            .ForCtorParam("DataInicio", opcao => opcao.MapFrom(origem => origem.DataInicio))
            .ForCtorParam("DataFim", opcao => opcao.MapFrom(origem => origem.DataFim))
            .ForCtorParam("VeiculoId", opcao => opcao.MapFrom(origem => origem.VeiculoId))
            .ForCtorParam("MecanicoId", opcao => opcao.MapFrom(origem => origem.MecanicoId));
    }
}
