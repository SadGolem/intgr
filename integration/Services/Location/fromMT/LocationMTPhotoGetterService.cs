using System.Security.Cryptography;
using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationMTPhotoGetterService : ServiceGetterBase<LocationMTPhotoDataResponse>, IGetterService<LocationMTPhotoDataResponse>
{
    private readonly IClientStorageService _clientStorage;
    private readonly MTconnectSettings _apiSettings;
    private readonly string _photoEndpointTemplate;
    private readonly ILogger<LocationMTPhotoGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private ILocationMTStorageService _storageService;
    private const string SAVEDIRECTORY = "C:\\Users\\zma20\\Downloads\\photo_2025-07-24_11-42-53.jpg";
    public LocationMTPhotoGetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationMTPhotoGetterService> logger,
        IAuthorizer authorizer, IOptions<AuthSettings> apiSettings,
        ILocationMTStorageService storageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect;
        _photoEndpointTemplate = _apiSettings.BaseUrl +
                                 _apiSettings.ApiClientSettings.LocationGetPhotoEndpoint;
        _storageService = storageService;
    }

    /*public async Task Get()
    {
        var locationsStatus = await base.Get(_httpClientFactory, _getEndpoint, false);

        if (locationsStatus != null)
        {
            await ProcessLocationsWithPhotos(locationsStatus);
            _storageService.Set(locationsStatus);
        }
    }*/
    //тест
    public async Task Get()
    {
        var location = new LocationMTPhotoDataResponse
        {
            datetime_create = DateTime.MinValue,
            idAPRO = 3395571,
            idMT = 17784
        };
        AddLocalImageToLocation(location, SAVEDIRECTORY);
            /*
        try
        {
            location.images = await DownloadLocationPhotos(16777215, _photoEndpointTemplate, false);

            if (location.images == null || location.images.Count == 0)
            {
                Console.WriteLine("No photos downloaded");
                return;
            }
            
            using var sha256 = SHA256.Create();
            
            for (int i = 0; i < location.images.Count; i++)
            {
                var imageBytes = location.images[i];
                var filePath = Path.Combine(SAVEDIRECTORY, $"location_{location.idMT}_photo_{i}.jpg");
                
                await File.WriteAllBytesAsync(filePath, imageBytes);
                
                byte[] hashBytes = sha256.ComputeHash(imageBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        
                Console.WriteLine($"Saved: {filePath}");
                Console.WriteLine($"SHA-256: {hashString}");
            }

            Console.WriteLine($"Successfully saved {location.images.Count} photos to: {SAVEDIRECTORY}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
       */ 
    }

    public void AddLocalImageToLocation(LocationMTPhotoDataResponse location, string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }
            
            byte[] imageBytes = File.ReadAllBytes(filePath);
            
            location.images ??= new List<byte[]>();
            
            location.images.Add(imageBytes);
        
            Console.WriteLine($"Successfully added image: {Path.GetFileName(filePath)} ({imageBytes.Length} bytes)");
            
            _storageService.Set(location);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding image to location: {ex.Message}");
            throw;
        }
    }
    private async Task ProcessLocationsWithPhotos(List<LocationMTPhotoDataResponse> locations)
    {
        foreach (var location in locations)
        {
            try
            {
                location.images = await DownloadLocationPhotos(location.idMT, _photoEndpointTemplate, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading photos for location {location.idMT}");
            }
        }
    }
}