using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class VolunteerService : IVolunteerService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<VolunteerService> _logger;

    public VolunteerService(IHttpClientFactory httpClientFactory, ILogger<VolunteerService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SarApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };
    }

    public async Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(PaginationRequest request)
    {
        try
        {
            var queryParams = $"?Page={request.Page}&PageSize={request.PageSize}&SearchText={Uri.EscapeDataString(request.SearchText ?? "")}";
            var response = await _httpClient.GetAsync($"/volunteers{queryParams}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new PaginatedResponse<VolunteerViewModel> { Items = [], TotalCount = 0 };
                
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<PaginationResponse<VolunteerViewModel>>(json, _jsonOptions)!;
            
            return new PaginatedResponse<VolunteerViewModel> 
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1, // Convert from 0-based to 1-based
                PageSize = request.PageSize
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error getting volunteers");
            throw new ApplicationException("API bağlantısı başarısız. Lütfen API sunucusunun çalıştığından emin olun.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting volunteers");
            throw new ApplicationException("Ekip üyeleri yüklenirken hata oluştu.");
        }
    }

    public async Task<VolunteerViewModel?> GetVolunteerByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/volunteers/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
            
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<VolunteerViewModel>(json, _jsonOptions);
    }

    public async Task<Guid> CreateVolunteerAsync(VolunteerCreateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/volunteers", content);
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonOptions);
                    if (errorResponse.TryGetProperty("detail", out var detail) && !string.IsNullOrEmpty(detail.GetString()))
                    {
                        throw new ApplicationException(detail.GetString());
                    }
                    else if (errorResponse.TryGetProperty("title", out var title) && !string.IsNullOrEmpty(title.GetString()))
                    {
                        throw new ApplicationException(title.GetString());
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, use raw content
                }
                throw new ApplicationException($"Validation Error: {errorContent}");
            }
            
            response.EnsureSuccessStatusCode();
            
            var location = response.Headers.Location?.ToString();
            var idString = location?.Split('/').LastOrDefault();
            return Guid.Parse(idString!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error creating volunteer");
            throw new ApplicationException("API bağlantısı başarısız.");
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            _logger.LogError(ex, "Error creating volunteer");
            throw new ApplicationException("Ekip üyesi oluşturulurken hata oluştu.");
        }
    }

    public async Task<bool> UpdateVolunteerAsync(Guid id, VolunteerUpdateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/volunteers/{id}", content);
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent, _jsonOptions);
                    if (errorResponse.TryGetProperty("detail", out var detail) && !string.IsNullOrEmpty(detail.GetString()))
                    {
                        throw new ApplicationException(detail.GetString());
                    }
                    else if (errorResponse.TryGetProperty("title", out var title) && !string.IsNullOrEmpty(title.GetString()))
                    {
                        throw new ApplicationException(title.GetString());
                    }
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, use raw content
                }
                throw new ApplicationException($"Validation Error: {errorContent}");
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error updating volunteer");
            throw new ApplicationException("API bağlantısı başarısız.");
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            _logger.LogError(ex, "Error updating volunteer");
            throw new ApplicationException("Ekip üyesi güncellenirken hata oluştu.");
        }
    }

    public async Task<bool> DeleteVolunteerAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/volunteers/{id}");
        return response.IsSuccessStatusCode;
    }
}
