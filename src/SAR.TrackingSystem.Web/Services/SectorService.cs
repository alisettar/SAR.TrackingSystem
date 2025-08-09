using SAR.TrackingSystem.Web.Models;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class SectorService : ISectorService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<SectorService> _logger;

    public SectorService(IHttpClientFactory httpClientFactory, ILogger<SectorService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SarApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<SectorViewModel>> GetSectorsAsync()
    {
        var response = await _httpClient.GetAsync("/sectors");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<SectorViewModel>>(json, _jsonOptions)!;
    }

    public async Task<SectorViewModel?> GetSectorByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/sectors/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SectorViewModel>(json, _jsonOptions);
    }

    public async Task<bool> CreateSectorAsync(SectorViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/sectors", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteSectorAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/sectors/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sector {SectorId}", id);
            return false;
        }
    }
}
