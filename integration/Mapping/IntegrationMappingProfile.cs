using System.Globalization;
using AutoMapper;
using integration.Context;
using integration.Context.MT;
using integration.Context.Request;
using integration.Context.Request.MT;

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
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.bik, opt => opt.MapFrom(src => 
                SafeParseLong(src.bik)))
            .ForMember(dest => dest.mailAddress, opt => opt.MapFrom(src => src.mailAddress))
            .ForMember(dest => dest.ogrn, opt => opt.MapFrom(src => 
                SafeParseLong(src.ogrn)))
            .ForMember(dest => dest.consumerType, opt => opt.MapFrom(src => 
                src.type_ka))
            .ForMember(dest => dest.idPerson, opt => opt.MapFrom(src => 
                SafeParseInt(src.person_id)))
            .ForMember(dest => dest.idBoss, opt => opt.MapFrom(src => src.boss.id))
            .ForMember(dest => dest.idBT, opt => opt.MapFrom(src => src.ext_id))
            .ForMember(dest => dest.idOrganization, opt => opt.MapFrom(src => src.root_company.id));

        CreateMap<EmitterDataResponse, EmitterRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.idConsumer, opt => opt.MapFrom(src => src.participant.id))
            .ForMember(dest => dest.idConsumerType, opt => opt.MapFrom(src => src.typeConsumer))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => SafeParseInt(src.amount)))
            .ForMember(dest => dest.consumerAddress, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.accountingType, opt => opt.MapFrom(src => src.normative))
            .ForMember(dest => dest.contractNumber, opt => opt.MapFrom(src => src.contractNumber))
            .ForMember(dest => dest.idLocation, opt => opt.MapFrom(src => src.location_mt_id))
            .ForMember(dest => dest.executorName, opt => opt.MapFrom(src => src.executorName))
            .ForMember(dest => dest.idContract, opt => opt.MapFrom(src => src.idContract))
            .ForMember(dest => dest.contractStatus, opt => opt.MapFrom(src => src.contractStatus))
            .ForMember(dest => dest.addressBT, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.usernameBT, opt => opt.MapFrom(src => src.nameConsumer));

        CreateMap<LocationDataResponse, LocationRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.status, opt => opt.MapFrom(src => (StatusCoder.ToCorrectLocationStatus(src.status.id, src.id))))
            .ForMember(dest => dest.latitude, opt => opt.MapFrom(src => 
                (double)Math.Round((decimal)src.lat, 5, MidpointRounding.ToZero)
            ))
            .ForMember(dest => dest.longitude, opt => opt.MapFrom(src => 
                (double)Math.Round((decimal)src.lon, 5, MidpointRounding.ToZero)
            ));


        CreateMap<ScheduleDataResponse, ScheduleRequest>()
            .ForMember(dest => dest.idWasteGenerator, opt => opt.MapFrom(src => src.emitter.id))
            .ForMember(dest => dest.idLocation, opt => opt.MapFrom(src => src.location.id))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.emitter.amount))
            .ForMember(dest => dest.idContainerType, opt => opt.MapFrom(src => src.idContainerType))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.LocationDataResponse.address))
            .ForMember(dest => dest.exportSchedule, opt => opt.MapFrom(src => src.gr_w));
        
        CreateMap<EntryDataResponse, EntryRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.BtNumber))
            .ForMember(dest => dest.idLocation, opt => opt.MapFrom(src => src.location.id))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.number ?? 0))
            .ForMember(dest => dest.idContainerType, opt => opt.MapFrom(src => src.idContainerType))
            .ForMember(dest => dest.volume, opt => opt.MapFrom(src => src.volume))
            .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.statusString))
            .ForMember(dest => dest.consumerName, opt => opt.MapFrom(src => src.location.client.name ?? ""))
            .ForMember(dest => dest.planDateRO, opt => opt.MapFrom(src => src.PlanDateRO))
            .ForMember(dest => dest.creationDate, opt => opt.MapFrom(src => src.datetime_create.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.type, opt => opt.MapFrom(src => "Заявка"))
            .ForMember(dest => dest.commentByRO, opt => opt.MapFrom(src => src.comment ?? ""))
            .ForMember(dest => dest.creator, opt => opt.MapFrom(src => src.Author.Name ?? ""));

        CreateMap<EntryMTDataResponse, EntryMTRequest>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.status_id, opt => opt.MapFrom(src => StatusCoder.GetStatusId(src.status ?? "")));
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
    
    decimal TruncateDecimal(decimal value, int precision)
    {
        decimal step = (decimal)Math.Pow(10, precision);
        decimal tmp = Math.Truncate(step * value);
        return tmp / step;
    }


}