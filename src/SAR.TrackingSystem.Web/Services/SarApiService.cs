using SAR.TrackingSystem.Web.Models;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class SarApiService : ISarApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<SarApiService> _logger;

    public SarApiService(IHttpClientFactory httpClientFactory, ILogger<SarApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SarApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            // Mock dashboard stats since API doesn't have this endpoint yet
            var volunteers = await GetVolunteersAsync(1, 1000);
            var movements = await GetMovementsAsync(1, 100);
            
            return new DashboardStats
            {
                TotalVolunteers = volunteers.TotalCount,
                InHubCount = 45,
                InSectorCount = 25,
                EntryCount = 15,
                ExitCount = 5
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return new DashboardStats { TotalVolunteers = 0 };
        }
    }

    public async Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/volunteers?page={page}&pageSize={pageSize}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new PaginatedResponse<VolunteerViewModel> { Items = [], TotalCount = 0 };
                
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<PaginationResponse<VolunteerViewModel>>(json, _jsonOptions)!;
            
            return new PaginatedResponse<VolunteerViewModel> 
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = page,
                PageSize = pageSize
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
            throw new ApplicationException("Gönüllüler yüklenirken hata oluştu.");
        }
    }

    public async Task<VolunteerViewModel?> GetVolunteerByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/volunteers/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
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
            throw new ApplicationException("Gönüllü oluşturulurken hata oluştu.");
        }
    }

    public async Task<bool> UpdateVolunteerAsync(Guid id, VolunteerUpdateViewModel model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PutAsync($"/volunteers/{id}", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteVolunteerAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"/volunteers/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<TeamViewModel>> GetTeamsAsync()
    {
        var response = await _httpClient.GetAsync("/teams");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TeamViewModel>>(json, _jsonOptions)!;
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

    public async Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/movements?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<PaginationResponse<MovementViewModel>>(json, _jsonOptions)!;
            
            return new PaginatedResponse<MovementViewModel> 
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movements");
            throw new ApplicationException("Hareketler yüklenirken hata oluştu.");
        }
    }

    public async Task<TeamViewModel?> GetTeamByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/teams/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TeamViewModel>(json, _jsonOptions);
    }

    public async Task<Guid> CreateMovementAsync(MovementCreateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/movements", content);
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Business Rule Error: {errorContent}");
            }
            
            response.EnsureSuccessStatusCode();
            
            var location = response.Headers.Location?.ToString();
            var idString = location?.Split('/').LastOrDefault();
            return Guid.Parse(idString!);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error creating movement");
            throw new ApplicationException("API bağlantısı başarısız.");
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            _logger.LogError(ex, "Error creating movement");
            throw new ApplicationException("Hareket kaydı yapılırken hata oluştu.");
        }
    }

    public async Task<bool> CreateSectorAsync(SectorViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/sectors", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateTeamAsync(TeamViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/teams", model);
        return response.IsSuccessStatusCode;
    }

}
