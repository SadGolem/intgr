using System.ComponentModel;
using static EmailMessageBuilder;

namespace integration.Services.Interfaces
{
    public interface IServiceBase
    {
        Task Authorize(HttpClient httpClient);
        void Message(string ex, ListType type);
    }
}
