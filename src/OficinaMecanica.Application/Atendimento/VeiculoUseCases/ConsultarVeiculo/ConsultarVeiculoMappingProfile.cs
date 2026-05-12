using AutoMapper;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoMappingProfile : Profile
{
    public ConsultarVeiculoMappingProfile()
    {
        CreateMap<Veiculo, ConsultarVeiculoResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("ClienteId", opcao => opcao.MapFrom(origem => origem.ClienteId))
            .ForCtorParam("Placa", opcao => opcao.MapFrom(origem => origem.Placa.NumeroPlaca))
            .ForCtorParam("Marca", opcao => opcao.MapFrom(origem => origem.Marca))
            .ForCtorParam("Modelo", opcao => opcao.MapFrom(origem => origem.Modelo))
            .ForCtorParam("Ano", opcao => opcao.MapFrom(origem => origem.Ano));
    }
}
