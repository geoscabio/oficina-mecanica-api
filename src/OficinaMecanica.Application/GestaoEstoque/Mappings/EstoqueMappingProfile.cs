using AutoMapper;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Application.GestaoEstoque.Mappings;

public sealed class EstoqueMappingProfile : Profile
{
    public EstoqueMappingProfile()
    {
        CreateMap<ItemEstoque, ItemEstoqueResponse>();
    }
}