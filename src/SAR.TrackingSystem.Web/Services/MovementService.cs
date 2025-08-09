using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class MovementService : IMovementService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<MovementService> _logger;
    private readonly IVolunteerService _volunteerService;
    private readonly ISectorService _sectorService;

    public MovementService(IHttpClientFactory httpClientFactory, ILogger<MovementService> logger, 
                          IVolunteerService volunteerService, ISectorService sectorService)
    {
        _httpClient = httpClientFactory.CreateClient("SarApi");
        _logger = logger;
        _volunteerService = volunteerService;
        _sectorService = sectorService;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(int page = 1, int pageSize = 10, string? search = null)
    {
        try
        {
            var paginationRequest = new PaginationRequest(page - 1, pageSize, search);
            var queryParams = $"?Page={paginationRequest.Page}&PageSize={paginationRequest.PageSize}&SearchText={Uri.EscapeDataString(paginationRequest.SearchText ?? "")}";
            var response = await _httpClient.GetAsync($"/movements{queryParams}");
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

    public async Task<List<MovementViewModel>> GetRecentMovementsAsync(int count = 5)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/movements?page=1&pageSize={count}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<PaginationResponse<MovementViewModel>>(json, _jsonOptions)!;
            
            return apiResponse.Items.Take(count).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent movements");
            return new List<MovementViewModel>();
        }
    }

    public async Task<Guid> CreateMovementAsync(MovementCreateViewModel model)
    {
        try
        {
            var json = JsonSerializer.Serialize(model, _jsonOptions);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/movements", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorResponse.TryGetProperty("detail", out var detail))
                    {
                        throw new ApplicationException(detail.GetString());
                    }
                    else if (errorResponse.TryGetProperty("title", out var title))
                    {
                        throw new ApplicationException(title.GetString());
                    }
                }
                catch (JsonException)
                {
                }
                
                var statusMessage = response.StatusCode == HttpStatusCode.BadRequest 
                    ? "Validation error" : "API error";
                throw new ApplicationException($"{statusMessage}: {errorContent}");
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

    public async Task<Guid> CreateTeamMovementAsync(TeamMovementCreateViewModel model)
    {
        // Team movement logic - needs ITeamService
        throw new NotImplementedException("Needs ITeamService dependency");
    }

    public async Task<bool> DeleteMovementAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/movements/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting movement {MovementId}", id);
            return false;
        }
    }

    public async Task<bool> CreateQuickEntryAsync(string qrId)
    {
        try
        {
            var volunteers = await _volunteerService.GetVolunteersAsync(1, 1000, qrId);
            var volunteer = volunteers.Items.FirstOrDefault(v => v.QRId == qrId);
            
            if (volunteer == null)
                return false;
                
            var sectors = await _sectorService.GetSectorsAsync();
            var entrySector = sectors.FirstOrDefault(s => s.Code == "ALAN_DIŞI");
            var hubSector = sectors.FirstOrDefault(s => s.Code == "BOO");
            
            if (entrySector == null || hubSector == null)
                return false;
                
            var movementModel = new MovementCreateViewModel
            {
                VolunteerId = volunteer.Id,
                FromSectorId = entrySector.Id,
                ToSectorId = hubSector.Id,
                Type = 0, // Entry
                IsGroupMovement = false,
                Notes = $"QR Giriş: {qrId}"
            };
            
            await CreateMovementAsync(movementModel);
            return true;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("400"))
        {
            _logger.LogWarning("Business rule validation failed for QR {QRId}: {Error}", qrId, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating quick entry for QR {QRId}", qrId);
            throw;
        }
    }
}
