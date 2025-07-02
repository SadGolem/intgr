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
    private readonly JsonSerializerOptions _jsonOptions;

    public ServiceGetterBase(
        IHttpClientFactory httpClientFactory,
        ILogger<ServiceBase> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings) 
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };
    }
    
    private async Task<HttpResponseMessage> ExecuteGetRequestAsync(string url, bool isApro)
    {
        var client = await Authorize(isApro);
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return response;
    }
    
    public async Task<List<T>> Get(string endpoint, bool isApro)
    {
        try
        {
            using var response = await ExecuteGetRequestAsync(endpoint, isApro);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(content, _jsonOptions) ?? new List<T>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while getting data from {endpoint}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while getting data from {endpoint}");
            throw;
        }
    }
    
    public async Task<TResult> GetFullResponse<TResult>(string endpoint, bool isApro)
    {
        try
        {
            using var response = await ExecuteGetRequestAsync(endpoint, isApro);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResult>(content, _jsonOptions) 
                ?? throw new InvalidOperationException("Deserialization returned null");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while getting full response from {endpoint}");
            throw;
        }
    }
    
    public async Task<List<byte[]>> DownloadLocationPhotos(int locationId, string endpointTemplate, bool isApro)
    {
        var photoUrl = endpointTemplate + locationId;
        
        try
        {
            using var response = await ExecuteGetRequestAsync(photoUrl, isApro);
            return await ProcessPhotoResponse(response, locationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Photo download failed for location {locationId}");
            return new List<byte[]>();
        }
    }

    // Обработка ответа с фотографиями
    private async Task<List<byte[]>> ProcessPhotoResponse(HttpResponseMessage response, int locationId)
    {
        var contentType = response.Content.Headers.ContentType?.MediaType;
        var contentDisposition = response.Content.Headers.ContentDisposition?.ToString();
        
        if (contentType?.StartsWith("multipart/") == true ||
            (contentDisposition?.Contains("filename=") == true && contentDisposition.Contains("attachment")))
        {
            return await HandleMultipartResponse(response, locationId);
        }
        
        if (contentType?.StartsWith("image/") == true)
        {
            return new List<byte[]> { await response.Content.ReadAsByteArrayAsync() };
        }
 
        _logger.LogWarning($"Unsupported content type for photos: {contentType}");
        return new List<byte[]>();
    }
    
    private async Task<List<byte[]>> HandleMultipartResponse(HttpResponseMessage response, int locationId)
    {
        if (response.Content.Headers.ContentDisposition?.FileName != null)
        {
            return new List<byte[]> { await response.Content.ReadAsByteArrayAsync() };
        }
        
        using var stream = await response.Content.ReadAsStreamAsync();
        var (boundary, combinedStream) = await TryExtractBoundary(stream);

        if (!string.IsNullOrEmpty(boundary))
        {
            return await ProcessMultipartContent(combinedStream, boundary, locationId);
        }
        
        _logger.LogWarning($"No boundary found for location {locationId}");
        return await ExtractPhotosFromZip(combinedStream);
    }
    
    private async Task<(string Boundary, Stream CombinedStream)> TryExtractBoundary(Stream stream)
    {
        var buffer = new byte[4096];
        int bytesRead = await stream.ReadAsync(buffer);

        if (bytesRead == 0) return (null, stream);

        string initialContent = Encoding.UTF8.GetString(buffer, 0, Math.Min(bytesRead, 128));
        var match = Regex.Match(initialContent, @"--([a-zA-Z0-9'()+_,-./:=?]{1,70})");

        if (!match.Success) return (null, stream);

        var boundary = match.Groups[1].Value;
        var combinedStream = new MemoryStream();
        combinedStream.Write(buffer, 0, bytesRead);
        await stream.CopyToAsync(combinedStream);
        combinedStream.Position = 0;

        return (boundary, combinedStream);
    }
    
    private async Task<List<byte[]>> ProcessMultipartContent(Stream stream, string boundary, int locationId)
    {
        var photos = new List<byte[]>();
        var reader = new MultipartReader(boundary, stream);
        
        try
        {
            MultipartSection section;
            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                if (section.ContentType?.StartsWith("image/") == true)
                {
                    using var ms = new MemoryStream();
                    await section.Body.CopyToAsync(ms);
                    photos.Add(ms.ToArray());
                }
            }
            return photos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing multipart for location {locationId}");
            return photos;
        }
    }
    private async Task<List<byte[]>> ExtractPhotosFromZip(Stream zipStream)
    {
        var photos = new List<byte[]>();
        
        using var archive = new ZipArchive(zipStream);
        foreach (var entry in archive.Entries)
        {
            if (!entry.Name.EndsWith(".jpg") && !entry.Name.EndsWith(".png")) continue;
            
            await using var entryStream = entry.Open();
            using var ms = new MemoryStream();
            await entryStream.CopyToAsync(ms);
            photos.Add(ms.ToArray());
        }
        return photos;
    }
}