using integration.Structs;

namespace integration.Services.CheckUp;

public interface ICheckUpService<T>
{
    bool Check(IntegrationStruct str);
}