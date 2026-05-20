using AutoMapper;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Application.Atendimento.Mappings;

public sealed class VeiculoMappingProfile : Profile
{
    public VeiculoMappingProfile()
    {
        CreateMap<Veiculo, VeiculoResponse>()
            .ForCtorParam(nameof(VeiculoResponse.Placa), opcao => opcao.MapFrom(origem => origem.Placa.NumeroPlaca));
    }
}


