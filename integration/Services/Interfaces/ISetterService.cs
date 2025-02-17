using integration.Context;
using System.ComponentModel;
using static EmailMessageBuilder;

namespace integration.Services.Interfaces
{
    public interface ISetterService<T> : IService
    {
        void PostAndPatch(T data, bool isNew);
        object MappingData(T data);
        bool Check(T data);
        void Message(EmailMessageBuilder.ListType type, string ex);
    }
}
