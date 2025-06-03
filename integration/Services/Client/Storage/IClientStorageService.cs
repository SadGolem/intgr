using integration.Context;
using integration.Services.Storage.Interfaces;

namespace integration.Services.Client.Storage;

public interface IClientStorageService : IStorageService<ClientDataResponse>
{
}