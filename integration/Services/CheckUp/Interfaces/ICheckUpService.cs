using integration.Structs;

namespace integration.Services.CheckUp;

public interface ICheckUpService<T>
{
    (bool, string) Check(IntegrationStruct str);
}