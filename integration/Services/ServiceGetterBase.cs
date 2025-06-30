using System.IO.Compression;
using System.Text.Json;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class ServiceGetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;

    public ServiceGetterBase(IHttpClientFactory httpClientFactory,
        ILogger<ServiceBase> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _logger = logger;
    }

    public async Task<List<T>> Get(IHttpClientFactory _httpClientFactory, string _connect, bool isApro)
    {
        var client = await Authorize(isApro);
        try
        {
            var response = await client.GetAsync(_connect);

            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContent = JsonSerializer.Deserialize<List<T>>(responseContentString);
            return responseContent;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while getting datas");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while getting datas");
            throw;
        }
    }
    public async Task<List<byte[]>> DownloadLocationPhotos(int locationId,string _endpoint, bool isApro)
    {
        var photos = new List<byte[]>();
        var photoUrl = string.Format(_endpoint, locationId);
        
        var client = await Authorize(isApro);
        
        var response = await client.GetAsync(photoUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Photo download failed for location {locationId}. Status: {response.StatusCode}");
            return photos;
        }

        // Определяем тип контента
        var contentType = response.Content.Headers.ContentType?.MediaType;
        
        // Обрабатываем разные форматы ответа
        if (contentType == "application/json")
        {
            // Если API возвращает JSON с массивом фото
            var jsonResponse = await response.Content.ReadFromJsonAsync<PhotoApiResponse>();
            photos = jsonResponse?.Images.Select(Convert.FromBase64String).ToList();
        }
        else if (contentType?.StartsWith("image/") == true)
        {
            // Если API возвращает непосредственно изображение
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            photos.Add(imageBytes);
        }
        else if (contentType == "application/zip")
        {
            // Если API возвращает архив с фото
            photos = await ExtractPhotosFromZip(await response.Content.ReadAsStreamAsync());
        }

        _logger.LogInformation($"Downloaded {photos.Count} photos for location {locationId}");
        return photos ?? new List<byte[]>();
    }
    private async Task<List<byte[]>> ExtractPhotosFromZip(Stream zipStream)
    {
        var photos = new List<byte[]>();
        
        using var archive = new ZipArchive(zipStream);
        foreach (var entry in archive.Entries.Where(e => e.Name.EndsWith(".jpg") || e.Name.EndsWith(".png")))
        {
            await using var entryStream = entry.Open();
            using var ms = new MemoryStream();
            await entryStream.CopyToAsync(ms);
            photos.Add(ms.ToArray());
        }
        
        return photos;
    }


    public async Task<T> GetFullResponse<T>(IHttpClientFactory _httpClientFactory, string _connect, bool isApro)
    {
        var client = await Authorize(isApro);
        var response = await client.GetAsync(_connect);
    
        response.EnsureSuccessStatusCode();
    
        var responseContentString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseContentString) ?? throw new InvalidOperationException("Deserialization failed");
    }
    public void Message(string ex)
    {
        throw new NotImplementedException();
    }
}