using integration.Context.MT;
using integration.Services.Storage.Interfaces;

namespace integration.Services.Agre.Storage;

public interface IAgreStorageService: IStorageService<(AgreData,int)>
{
    
}