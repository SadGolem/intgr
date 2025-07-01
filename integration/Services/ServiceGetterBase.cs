using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
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

    public async Task<List<byte[]>> DownloadLocationPhotos(int locationId, string _endpoint, bool isApro)
    {
        var photos = new List<byte[]>();
        var photoUrl = _endpoint + locationId;

        var client = await Authorize(isApro);
        var response = await client.GetAsync(photoUrl);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Photo download failed for location {locationId}. Status: {response.StatusCode}");
            return photos;
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;

        // Проверяем Content-Disposition как резервный вариант
        var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
        
        if (contentType?.StartsWith("multipart/") == true ||
                 (contentDisposition?.Contains("filename=") == true && contentDisposition.Contains("attachment")))
        {
            // Комбинированный подход для обработки multipart без boundary
            return await HandleMultipartWithoutBoundary(response, locationId);
        }

        return photos;
    }

    private async Task<List<byte[]>> HandleMultipartWithoutBoundary(HttpResponseMessage response, int locationId)
    {
        var photos = new List<byte[]>();

        try
        {
            // Вариант 1: Попробуем прочитать как одиночное изображение
            if (response.Content.Headers.ContentDisposition?.FileName != null)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                photos.Add(imageBytes);
                _logger.LogInformation($"Read single image from multipart response for location {locationId}");
                return photos;
            }

            // Вариант 2: Попытаемся найти boundary в теле ответа
            using var stream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead > 0)
            {
                // Поиск boundary в первых байтах контента
                string initialContent = Encoding.UTF8.GetString(buffer, 0, Math.Min(bytesRead, 128));
                var boundaryMatch = Regex.Match(initialContent, @"--([a-zA-Z0-9'()+_,-./:=?]{1,70})");

                if (boundaryMatch.Success)
                {
                    string boundary = boundaryMatch.Groups[1].Value;
                    _logger.LogInformation($"Extracted boundary from body: {boundary} for location {locationId}");

                    // Объединяем прочитанный буфер с остатком потока
                    var combinedStream = new MemoryStream();
                    combinedStream.Write(buffer, 0, bytesRead);
                    await stream.CopyToAsync(combinedStream);
                    combinedStream.Position = 0;

                    // Обработка с найденным boundary
                    return await ProcessMultipartWithBoundary(combinedStream, boundary, locationId);
                }
            }

            // Вариант 3: Если boundary не найден, пробуем обработать как архив
            _logger.LogWarning($"No boundary found, trying to process as ZIP for location {locationId}");
            stream.Position = 0;
            return await ExtractPhotosFromZip(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error handling multipart without boundary for location {locationId}");
            return photos;
        }
    }

    private async Task<List<byte[]>> ProcessMultipartWithBoundary(Stream stream, string boundary, int locationId)
    {
        var photos = new List<byte[]>();
        var reader = new MultipartReader(boundary, stream);
        MultipartSection section;

        try
        {
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (section.ContentType?.StartsWith("image/") == true)
                {
                    using var memoryStream = new MemoryStream();
                    await section.Body.CopyToAsync(memoryStream);
                    photos.Add(memoryStream.ToArray());
                }
            }

            _logger.LogInformation(
                $"Processed {photos.Count} images with extracted boundary for location {locationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing multipart with extracted boundary for location {locationId}");
        }

        return photos;
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
        return JsonSerializer.Deserialize<T>(responseContentString) ??
               throw new InvalidOperationException("Deserialization failed");
    }

    public void Message(string ex)
    {
        throw new NotImplementedException();
    }
}