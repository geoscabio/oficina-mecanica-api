using AutoMapper;
using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public sealed class ConsultarClientePorDocumentoMappingProfile : Profile
{
    public ConsultarClientePorDocumentoMappingProfile()
    {
        CreateMap<Cliente, ConsultarClientePorDocumentoResponse>()
            .ForCtorParam("Id", opcao => opcao.MapFrom(origem => origem.Id))
            .ForCtorParam("Documento", opcao => opcao.MapFrom(origem => origem.Documento.Numero))
            .ForCtorParam("Nome", opcao => opcao.MapFrom(origem => origem.Nome))
            .ForCtorParam("Logradouro", opcao => opcao.MapFrom(origem => origem.Endereco.Logradouro))
            .ForCtorParam("Numero", opcao => opcao.MapFrom(origem => origem.Endereco.Numero))
            .ForCtorParam("Bairro", opcao => opcao.MapFrom(origem => origem.Endereco.Bairro))
            .ForCtorParam("Cidade", opcao => opcao.MapFrom(origem => origem.Endereco.Cidade))
            .ForCtorParam("CEP", opcao => opcao.MapFrom(origem => origem.Endereco.CEP))
            .ForCtorParam("Telefone", opcao => opcao.MapFrom(origem => origem.Telefone.Numero))
            .ForCtorParam("Email", opcao => opcao.MapFrom(origem => origem.Email.Endereco));
    }
}
