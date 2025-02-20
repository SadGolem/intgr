using integration.Context;
using System.ComponentModel;
using static EmailMessageBuilder;

namespace integration.Services.Interfaces
{
    public interface ISetterService<T> : IService
    {
        Task PostAndPatch(List<(T,bool)> data);
        Task Post(T data);
        Task Patch(T data);
        object MappingData(T data);
        bool Check(T data);
    }
}
