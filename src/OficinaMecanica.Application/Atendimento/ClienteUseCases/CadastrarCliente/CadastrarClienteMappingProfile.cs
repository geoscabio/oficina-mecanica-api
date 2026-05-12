using AutoMapper;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed class CadastrarClienteMappingProfile : Profile
{
    public CadastrarClienteMappingProfile()
    {
        CreateMap<Cliente, CadastrarClienteResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("Documento", opcao => opcao.MapFrom(origem => origem.Documento.Numero))
            .ForCtorParam("Nome", opcao => opcao.MapFrom(origem => origem.Nome))
            .ForCtorParam("Email", opcao => opcao.MapFrom(origem => origem.Email.Endereco));
    }
}
