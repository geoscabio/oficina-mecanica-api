using AutoMapper;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.Administrativo.Mappings;

public sealed class MecanicoMappingProfile : Profile
{
    public MecanicoMappingProfile()
    {
        CreateMap<Mecanico, MecanicoResponse>();
    }
}