using AutoMapper;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed class IniciarDiagnosticoOrdemServicoMappingProfile : Profile
{
    public IniciarDiagnosticoOrdemServicoMappingProfile()
    {
        CreateMap<OrdemServico, IniciarDiagnosticoOrdemServicoResponse>()
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
