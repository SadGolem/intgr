using System.Text.RegularExpressions;
using integration.Context;
using integration.Services.Integration.Interfaces;

namespace integration.Services.Integration.Processors;

public class BaseProcessor : IIntegrationProcessor<DataResponse>
{
    public Task ProcessAsync(DataResponse entity)
    {
        throw new NotImplementedException();
    }
    
    public int? ParseMtIdFromResponse(string response)
    {
        var match = Regex.Match(response, @"id (\d+)$");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
        {
            return id;
        }
        return null;
    }
}