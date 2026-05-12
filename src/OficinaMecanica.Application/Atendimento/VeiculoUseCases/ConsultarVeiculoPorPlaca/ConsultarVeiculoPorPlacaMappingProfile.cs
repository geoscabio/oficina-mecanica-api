using AutoMapper;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public sealed class ConsultarVeiculoPorPlacaMappingProfile : Profile
{
    public ConsultarVeiculoPorPlacaMappingProfile()
    {
        CreateMap<Veiculo, ConsultarVeiculoPorPlacaResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("ClienteId", opcao => opcao.MapFrom(origem => origem.ClienteId))
            .ForCtorParam("Placa", opcao => opcao.MapFrom(origem => origem.Placa.NumeroPlaca))
            .ForCtorParam("Marca", opcao => opcao.MapFrom(origem => origem.Marca))
            .ForCtorParam("Modelo", opcao => opcao.MapFrom(origem => origem.Modelo))
            .ForCtorParam("Ano", opcao => opcao.MapFrom(origem => origem.Ano));
    }
}
