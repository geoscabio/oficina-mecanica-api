using AutoMapper;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.Administrativo.Mappings;

public sealed class PecaInsumoCatalogoMappingProfile : Profile
{
    public PecaInsumoCatalogoMappingProfile()
    {
        CreateMap<PecaInsumoCatalogo, PecaInsumoCatalogoResponse>();
    }
}