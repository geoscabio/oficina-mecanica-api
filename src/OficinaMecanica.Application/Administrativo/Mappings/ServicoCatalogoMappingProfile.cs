using AutoMapper;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.Administrativo.Mappings;

public sealed class ServicoCatalogoMappingProfile : Profile
{
    public ServicoCatalogoMappingProfile()
    {
        CreateMap<ServicoCatalogo, ServicoCatalogoResponse>();
    }
}
