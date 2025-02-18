using integration.Context;
using System.ComponentModel;
using static EmailMessageBuilder;

namespace integration.Services.Interfaces
{
    public interface ISetterService<T> : IService
    {
        Task PostOrPatch(List<T> data);
        object MappingData(T data);
        bool Check(T data);
    }
}
