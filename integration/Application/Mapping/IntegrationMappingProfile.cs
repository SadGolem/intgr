using AutoMapper;
using integration.Context;
using integration.Context.MT;
using integration.Context.Request;
using integration.Context.Request.MT;
using integration.Domain.Entities;

public class IntegrationMappingProfile : Profile
{
    public IntegrationMappingProfile()
    {
        CreateMap<ClientDataResponse, ClientRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.idAsuPro))
            .ForMember(dest => dest.consumerName, opt => opt.MapFrom(src => src.consumerName))
            .ForMember(dest => dest.inn, opt => opt.MapFrom(src => SafeParseLong(src.inn)))
            .ForMember(dest => dest.kpp, opt => opt.MapFrom(src => SafeParseInt(src.kpp)))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.bik, opt => opt.MapFrom(src => SafeParseLong(src.bik)))
            .ForMember(dest => dest.mailAddress, opt => opt.MapFrom(src => src.mailAddress))
            .ForMember(dest => dest.ogrn, opt => opt.MapFrom(src => SafeParseLong(src.ogrn)))
            .ForMember(dest => dest.consumerType, opt => opt.MapFrom(src => src.type_ka))
            .ForMember(dest => dest.idPerson, opt => opt.MapFrom(src => SafeParseInt(src.person_id)))
            .ForMember(dest => dest.idBoss, opt => opt.MapFrom(src =>
                src.boss != null ? src.boss.id : 0))
            /*.ForMember(dest => dest.idBT, opt => opt.MapFrom(src =>
                SafeParseInt(src.ext_id)))*/
            .ForMember(dest => dest.idOrganization, opt => opt.MapFrom(src =>
                src.root_company != null ? src.root_company.id : 0));

        CreateMap<EmitterDataResponse, EmitterRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.WasteSource.id))
            .ForMember(dest => dest.idConsumer, opt => opt.MapFrom(src => src.participant_id))
            .ForMember(dest => dest.idConsumerType, opt => opt.MapFrom(src => src.typeConsumer))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.amount))
            .ForMember(dest => dest.consumerAddress, opt => opt.MapFrom(src => src.WasteSource.address))
            .ForMember(dest => dest.accountingType, opt => opt.MapFrom(src => src.WasteSource.normative))
            .ForMember(dest => dest.contractNumber, opt => opt.MapFrom(src => src.contractNumber))
            .ForMember(dest => dest.idLocation, opt => opt.MapFrom(src => src.location_mt_id))
            .ForMember(dest => dest.executorName, opt => opt.MapFrom(src => src.executorName))
            .ForMember(dest => dest.idContract, opt => opt.MapFrom(src => src.idContract))
            .ForMember(dest => dest.contractStatus, opt => opt.MapFrom(src => src.contractStatus))
            .ForMember(dest => dest.addressBT, opt => opt.MapFrom(src => src.WasteSource.address))
            .ForMember(dest => dest.usernameBT, opt => opt.MapFrom(src => src.nameConsumer));

        CreateMap<LocationDataResponse, LocationRequest>()
            .ForMember(dest => dest.idAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.status,
                opt => opt.MapFrom(src => (StatusCoder.ToCorrectLocationStatus(src.status.id, src.id))))
            .ForMember(dest => dest.latitude, opt => opt.MapFrom(src =>
                (double)Math.Round((decimal)src.lat, 5, MidpointRounding.ToZero)
            ))
            .ForMember(dest => dest.longitude, opt => opt.MapFrom(src =>
                (double)Math.Round((decimal)src.lon, 5, MidpointRounding.ToZero)
            ));

        CreateMap<LocationDataResponse, LocationEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src =>
                src.address != null
                    ? src.address.Substring(0, Math.Min(src.address.Length, 500))
                    : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                StatusCoder.ToCorrectLocationStatus(src.status.id, src.id) != null
                    ? StatusCoder.ToCorrectLocationStatus(src.status.id, src.id)
                    : "UNKNOWN"
            ))
            .ForMember(dest => dest.ClientIdAsuPro, opt => opt.MapFrom(src => 
                src.client != null ? src.client.id : (int?)null))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src =>
                Math.Round(src.lat, 6)))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src =>
                Math.Round(src.lon, 6)))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src =>
                src.comment != null
                    ? src.comment.Substring(0, Math.Min(src.comment.Length, 1000))
                    : null))
            .ForMember(dest => dest.IdParticipant, opt => opt.MapFrom(src =>
                src.participant != null ? src.participant.id : (int?)null))
            .ForMember(dest => dest.AuthorUpdate, opt => opt.MapFrom(src =>
                src.author_update != null
                    ? src.author_update.Substring(0, Math.Min(src.author_update.Length, 100))
                    : null))
            .ForMember(dest => dest.ExtId, opt => opt.MapFrom(src =>
                src.ext_id != null
                    ? src.ext_id.Substring(0, Math.Min(src.ext_id.Length, 100))
                    : null));

        CreateMap<ClientDataResponse, ClientEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
           // .ForMember(dest => dest.ClientId, opt => opt.Ignore())
            .ForMember(dest => dest.IdAsuPro, opt => opt.MapFrom(src => src.idAsuPro))
            .ForMember(dest => dest.Bik, opt => opt.MapFrom(src => src.bik))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.address))
            .ForMember(dest => dest.Inn, opt => opt.MapFrom(src => src.inn))
            .ForMember(dest => dest.Boss, opt => opt.MapFrom(src => src.boss))
            .ForMember(dest => dest.Doc_type, opt => opt.MapFrom(src => src.doc_type))
            .ForMember(dest => dest.Ext_id, opt => opt.MapFrom(src => src.ext_id))
            .ForMember(dest => dest.Kpp, opt => opt.MapFrom(src => src.kpp))
            .ForMember(dest => dest.Ogrn, opt => opt.MapFrom(src => src.ogrn))
            .ForMember(dest => dest.Person_id, opt => opt.MapFrom(src => src.person_id))
            .ForMember(dest => dest.ConsumerName, opt => opt.MapFrom(src => src.consumerName))
            .ForMember(dest => dest.MailAddress, opt => opt.MapFrom(src => src.mailAddress))
            .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.shortName))
            .ForMember(dest => dest.Root_company, opt => opt.MapFrom(src => src.root_company));

        CreateMap<EmitterDataResponse, EmitterEntity>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src =>
                src.amount))
            .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.contractNumber))
            .ForMember(dest => dest.Location_Mt_Id, opt => opt.MapFrom(src => src.location_mt_id))
            .ForMember(dest => dest.ExecutorName, opt => opt.MapFrom(src => src.executorName))
            .ForMember(dest => dest.IdContract, opt => opt.MapFrom(src => src.idContract))
            .ForMember(dest => dest.ContractStatus, opt => opt.MapFrom(src => src.contractStatus))
            .ForMember(dest => dest.Participant_Id, opt => opt.MapFrom(src => src.participant_id))
            .ForMember(dest => dest.TypeConsumer, opt => opt.MapFrom(src => src.typeConsumer))
            .ForMember(dest => dest.NameConsumer, opt => opt.MapFrom(src => src.nameConsumer))
            .ForMember(dest => dest.IdConsumer, opt => opt.MapFrom(src => src.idConsumer))
            .ForMember(dest => dest.WasteSource_Id, opt => opt.MapFrom(src => src.WasteSource.id))
            .ForMember(dest => dest.WasteSource_Address, opt => opt.MapFrom(src =>
                src.WasteSource.address))
            .ForMember(dest => dest.WasteSource_Name, opt => opt.MapFrom(src =>
                src.WasteSource.name))
            .ForMember(dest => dest.WasteSource_Category, opt => opt.MapFrom(src =>
                src.WasteSource.category.name))
            .ForMember(dest => dest.WasteSource_Normative, opt => opt.MapFrom(src => src.WasteSource.normative))
            .ForMember(dest => dest.WasteSource_Ext_id, opt => opt.MapFrom(src =>
                src.WasteSource.ext_id))
            .ForMember(dest => dest.Containers_IDs, opt => opt.MapFrom(src =>
                src.container != null
                    ? src.container.Select(c => c.id).ToList()
                    : null));

        CreateMap<ScheduleDataResponse, ScheduleEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IdAsuPro, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.IdLocation, opt => opt.MapFrom(src => src.location.id))
            .ForMember(dest => dest.Schedule, opt => opt.MapFrom(src => src.gr_w))
            .ForMember(dest => dest.Dates, opt => opt.MapFrom(src => src.dates))
            .ForMember(dest => dest.Ext_id, opt => opt.MapFrom(src => src.ext_id))
            .ForMember(dest => dest.idContainerType, opt => opt.MapFrom(src => src.idContainerType))
            .ForMember(dest => dest.Containers_IDs, opt => opt.MapFrom(src =>
                src.containers != null
                    ? src.containers.Select(c => c.id).ToList()
                    : null))
            .ForMember(dest => dest.IdEmitter, opt => opt.MapFrom(src =>
                src.emitter != null ? src.emitter.WasteSource.id : (int?)null));

        CreateMap<ScheduleDataResponse, ScheduleRequest>()
            .ForMember(dest => dest.idWasteGenerator, opt => opt.MapFrom(src => src.emitter.WasteSource.ext_id))
            .ForMember(dest => dest.idLocation, opt => opt.MapFrom(src => src.location.id))
            .ForMember(dest => dest.amount, opt => opt.MapFrom(src => src.emitter.container.Count))
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

        CreateMap<EntryDataResponse, EntryEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsAsuPro, opt => opt.MapFrom(src => src.BtNumber))
            .ForMember(dest => dest.PlanDateRO, opt => opt.MapFrom(src =>
                src.PlanDateRO))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src =>
                src.Author != null ? src.Author.Name : null))
            .ForMember(dest => dest.IdLocation, opt => opt.MapFrom(src => src.location.id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.status))
            .ForMember(dest => dest.Agreement, opt => opt.MapFrom(src =>
                src.agreement != null ? src.agreement.id.ToString() : null))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src =>
                src.comment))
            .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.volume))
            .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src =>
                src.Capacity != null ? src.Capacity.volume : null))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.number))
            .ForMember(dest => dest.IdContainerType, opt => opt.MapFrom(src =>
                src.Capacity != null && src.Capacity.type != null
                    ? src.Capacity.type.id
                    : src.idContainerType));

        CreateMap<EntryData, EntryMTRequest>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.status_id, opt => opt.MapFrom(src => StatusCoder.GetStatusId(src.status)));

        CreateMap<LocationMTPhotoDataResponse, LocationMTPhotoRequest>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.idAPRO))
            .ForMember(dest => dest.photos, opt => opt.MapFrom(src => src.images));

        CreateMap<LocationData, LocationMTStatusRequest>()
            .ForMember(dest => dest.status_id,
                opt => opt.MapFrom(src => StatusCoder.FromCorrectLocationStatus(src.status)));

        CreateMap<AgreData, AgreRequest>()
            .ForMember(dest => dest.comment_disp, opt => opt.MapFrom(src => $"{src.username}: {src.comment}"));
    }

    private static long SafeParseLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return long.TryParse(value, out var result) ? result : 0;
    }

    private static int SafeParseInt(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return int.TryParse(value, out var result) ? result : 0;
    }

    private double SafeParseDouble(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        return double.TryParse(value, out double result) ? result : 0;
    }
}