using AutoMapper;

namespace integration.Context.Request;
using AutoMapper;

public class ClientRequest
{
    public int idAsuPro { get; set; }
    public string consumerType { get; set; }
    public string consumerName { get; set; }
    public long inn { get; set; }
    public int kpp { get; set; }
    public int idBoss { get; set; }
    public int idPerson { get; set; }
    public string address { get; set; }
    public long bik { get; set; }
    public string mailAddress { get; set; }
    public int idBT { get; set; }
    public int idOrganization { get; set; }
    public long ogrn { get; set; }


public ClientRequest IntegrationMappingProfile()
{
    CreateMap<ClientDataResponse, ClientRequest>()
        .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.idAsuPro))
        .ForMember(dest => dest.consumerName, opt => opt.MapFrom(src => src.consumerName))
        .ForMember(dest => dest.inn, opt => opt.ConvertUsing(new StringToLongConverter(), src => src.inn))
        .ForMember(dest => dest.kpp, opt => opt.ConvertUsing(new StringToIntConverter(), src => src.kpp))
        .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.mailAddress))
        .ForMember(dest => dest.bik, opt => opt.ConvertUsing(new StringToLongConverter(), src => src.bik))
        .ForMember(dest => dest.mailAddress, opt => opt.MapFrom(src => src.mailAddress))
        .ForMember(dest => dest.ogrn, opt => opt.ConvertUsing(new StringToLongConverter(), src => src.ogrn))
        .ForMember(dest => dest.consumerType, opt => opt.MapFrom(src = src.))
        .ForMember(dest => dest.idBoss, opt => opt.Ignore())
        .ForMember(dest => dest.idPerson, opt => opt.Ignore())
        .ForMember(dest => dest.idBT, opt => opt.Ignore())
        .ForMember(dest => dest.idOrganization, opt => opt.Ignore());
}
}