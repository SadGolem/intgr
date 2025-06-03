using integration.Context;

namespace integration.Services.Emitter.Storage;

public class EmitterStorageService : IEmitterStorageService
{
    public static List<EmitterDataResponse> EmitterPositionList = new List<EmitterDataResponse>();

    public List<EmitterDataResponse> Get()
    {
        return EmitterPositionList;
    }

    public void Set(EmitterDataResponse dates)
    {
        EmitterPositionList.Add(dates);
    }

    public void Set(List<EmitterDataResponse> dates)
    {
        foreach (var data in dates)
        {
            EmitterPositionList.Add(data);
        }
    }

    public void ClearList(EmitterDataResponse date)
    {
        EmitterPositionList.Remove(date);
    }

    public void ClearList()
    {
        EmitterPositionList.Clear();
    }
}