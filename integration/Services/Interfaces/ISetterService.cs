
namespace integration.Services.Interfaces
{
    public interface ISetterService<T> : IService
    {
        public Task Set();
    }
}
