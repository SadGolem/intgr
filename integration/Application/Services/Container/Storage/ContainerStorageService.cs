namespace integration.Services.Container.Storage;

public class ContainerStorageService : IContainerStorageService
{
    public static List<Context.Container> _containerDatas;
    public List<Context.Container> Get()
    {
        return _containerDatas;
    }

    public void Set(Context.Container data)
    {
        _containerDatas.Add(data);
    }

    public void Set(List<Context.Container> datas)
    {
        _containerDatas = datas;
    }

    public void ClearList(Context.Container data)
    {
        _containerDatas.Remove(data);
    }

    public void ClearList()
    {
        _containerDatas.Clear();
    }
}