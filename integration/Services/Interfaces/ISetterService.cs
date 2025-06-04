using integration.Context;
using System.ComponentModel;
using static EmailMessageBuilder;

namespace integration.Services.Interfaces
{
    public interface ISetterService<T> : IService
    {
        Task PostAndPatch();
    }
}
