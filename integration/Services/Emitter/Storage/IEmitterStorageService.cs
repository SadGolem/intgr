using integration.Context;
using integration.Services.Storage.Interfaces;

namespace integration.Services.Emitter.Storage;

public interface IEmitterStorageService : IStorageService<EmitterDataResponse>
{
}