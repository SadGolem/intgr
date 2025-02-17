using integration.Services.Interfaces;

namespace integration.Services
{
    public abstract class ServiceBase : IServiceBase
    {
        public virtual async Task Authorize(HttpClient httpClient)
        {
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void Check()
        {
            throw new NotImplementedException();
        }
    }
}
