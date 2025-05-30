using AutoMapper;
using integration.Context;
using integration.Context.Request;

public class IntegrationMappingProfile : Profile
{
    public IntegrationMappingProfile()
    {
        CreateMap<ClientDataResponse, ClientRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.idAsuPro))
            .ForMember(dest => dest.consumerName, opt => opt.MapFrom(src => src.consumerName))
            .ForMember(dest => dest.inn, opt => opt.MapFrom(src => 
                SafeParseLong(src.inn)))
            .ForMember(dest => dest.kpp, opt => opt.MapFrom(src => 
                SafeParseInt(src.kpp)))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.mailAddress))
            .ForMember(dest => dest.bik, opt => opt.MapFrom(src => 
                SafeParseLong(src.bik)))
            .ForMember(dest => dest.mailAddress, opt => opt.MapFrom(src => src.mailAddress))
            .ForMember(dest => dest.ogrn, opt => opt.MapFrom(src => 
                SafeParseLong(src.ogrn)))
            .ForMember(dest => dest.consumerType, opt => opt.MapFrom(src => 
                src.doc_type != null ? src.doc_type.name : null))
            .ForMember(dest => dest.idPerson, opt => opt.MapFrom(src => 
                SafeParseInt(src.person_id)))
            .ForMember(dest => dest.idBoss, opt => opt.MapFrom(src => src.boss.id))
            .ForMember(dest => dest.idBT, opt => opt.MapFrom(src => src.ext_id))
            .ForMember(dest => dest.idOrganization, opt => opt.MapFrom(src => src.root_company.id));
        
        CreateMap<EmitterDataResponse, EmitterRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.idConsumer, opt => opt.MapFrom(src => src.client.id))
            .ForMember(dest => dest.idConsumerType, opt => opt.MapFrom(src => src.id))
    }

    private long SafeParseLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return long.TryParse(value, out long result) ? result : 0;
    }

    private int SafeParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return int.TryParse(value, out int result) ? result : 0;
    }
}