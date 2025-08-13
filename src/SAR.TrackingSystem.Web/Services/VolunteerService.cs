using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services.Interfaces;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class VolunteerService(
    IHttpClientFactory httpClientFactory,
    ILogger<VolunteerService> logger) : IVolunteerService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("SarApi");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    public async Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(PaginationRequest request, string? filter = null)
    {
        try
        {
            var filterParam = !string.IsNullOrEmpty(filter) ? $"&filter={Uri.EscapeDataString(filter)}" : "";
            var queryParams = $"?paginationRequest={{\"SearchText\":\"{Uri.EscapeDataString(request.SearchText ?? "")}\",\"Page\":{request.Page},\"PageSize\":{request.PageSize}}}{filterParam}";
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
            logger.LogError(ex, "HTTP error getting volunteers");
            throw new ApplicationException("API bağlantısı başarısız. Lütfen API sunucusunun çalıştığından emin olun.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting volunteers");
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

    public async Task<VolunteerViewModel?> GetVolunteerByQRIdAsync(string qrid)
    {
        var response = await _httpClient.GetAsync($"/volunteers/qrid/{qrid}");
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
            logger.LogError(ex, "HTTP error creating volunteer");
            throw new ApplicationException("API bağlantısı başarısız.");
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            logger.LogError(ex, "Error creating volunteer");
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
            logger.LogError(ex, "HTTP error updating volunteer");
            throw new ApplicationException("API bağlantısı başarısız.");
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            logger.LogError(ex, "Error updating volunteer");
            throw new ApplicationException("Ekip üyesi güncellenirken hata oluştu.");
        }
    }

    public async Task<bool> DeleteVolunteerAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/volunteers/{id}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<MovementHistoryViewModel>> GetVolunteerMovementHistoryAsync(Guid id)
    {
        try
        {
            var cacheBuster = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var response = await _httpClient.GetAsync($"/volunteers/{id}/movements?_t={cacheBuster}");
            
            if (!response.IsSuccessStatusCode)
                return [];
            
            var content = await response.Content.ReadAsStringAsync();
            var apiData = JsonSerializer.Deserialize<List<MovementHistoryApiModel>>(content, _jsonOptions);
            
            return apiData?.Select(m => new MovementHistoryViewModel
            {
                Id = m.Id,
                MovementTime = m.MovementTime,
                FromSector = m.FromSector,
                ToSector = m.ToSector,
                MovementType = m.MovementType,
                IsGroupMovement = m.IsGroupMovement,
                Notes = m.Notes
            }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting movement history for volunteer {Id}", id);
            return [];
        }
    }

    public async Task<List<Guid>> GetTeamIdsWithNonArrivedVolunteersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/volunteers/teams-with-non-arrived");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Guid>>(json, _jsonOptions) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting team IDs with non-arrived volunteers");
            return [];
        }
    }
}

public record MovementHistoryApiModel(
    Guid Id,
    DateTime MovementTime,
    string FromSector,
    string ToSector,
    string MovementType,
    bool IsGroupMovement,
    string Notes
);
