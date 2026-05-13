using AutoMapper;
using OficinaMecanica.Application.Atendimento.Responses;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.Mappings;

public sealed class ClienteMappingProfile : Profile
{
    public ClienteMappingProfile()
    {
        CreateMap<Endereco, EnderecoResponse>()
            .ForCtorParam("Logradouro", opcao => opcao.MapFrom(origem => origem.Logradouro))
            .ForCtorParam("Numero", opcao => opcao.MapFrom(origem => origem.Numero))
            .ForCtorParam("Bairro", opcao => opcao.MapFrom(origem => origem.Bairro))
            .ForCtorParam("Cidade", opcao => opcao.MapFrom(origem => origem.Cidade))
            .ForCtorParam("CEP", opcao => opcao.MapFrom(origem => origem.CEP));

        CreateMap<Cliente, ClienteResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("Documento", opcao => opcao.MapFrom(origem => origem.Documento.Numero))
            .ForCtorParam("Nome", opcao => opcao.MapFrom(origem => origem.Nome))
            .ForCtorParam("Endereco", opcao => opcao.MapFrom(origem => origem.Endereco))
            .ForCtorParam("Telefone", opcao => opcao.MapFrom(origem => origem.Telefone.Numero))
            .ForCtorParam("Email", opcao => opcao.MapFrom(origem => origem.Email.Endereco));
    }
}
