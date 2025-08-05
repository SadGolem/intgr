namespace integration.Services.CheckUp;

public interface ICheckUpFactory<T> where T : class
{
    ICheckUpService<T> Create();
}