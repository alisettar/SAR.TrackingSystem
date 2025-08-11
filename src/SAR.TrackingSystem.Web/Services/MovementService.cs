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

    public async Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(PaginationRequest request)
    {
        try
        {
            var queryParams = $"?Page={request.Page}&PageSize={request.PageSize}&SearchText={Uri.EscapeDataString(request.SearchText ?? "")}";
            var response = await _httpClient.GetAsync($"/movements{queryParams}");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<PaginationResponse<MovementViewModel>>(json, _jsonOptions)!;
            
            return new PaginatedResponse<MovementViewModel> 
            {
                Items = apiResponse.Items,
                TotalCount = apiResponse.TotalCount,
                Page = request.Page + 1, // Convert from 0-based to 1-based
                PageSize = request.PageSize
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
        try
        {
            // Get team members
            var teamMembers = await GetTeamMembersAsync(model.TeamId);
            _logger.LogInformation("Found {Count} team members for team {TeamId}", teamMembers.Count, model.TeamId);
            
            if (teamMembers.Count == 0)
                throw new ApplicationException("Seçilen ekipte üye bulunamadı.");

            var groupId = Guid.NewGuid(); // Single group ID for all movements
            var successCount = 0;
            var errors = new List<string>();

            // Create movement for each team member
            foreach (var member in teamMembers)
            {
                try
                {
                    _logger.LogInformation("Creating movement for member {MemberName} ({MemberId})", member.FullName, member.Id);
                    
                    var movementModel = new MovementCreateViewModel
                    {
                        VolunteerId = member.Id,
                        FromSectorId = model.FromSectorId,
                        ToSectorId = model.ToSectorId,
                        Type = 1, // Transfer
                        IsGroupMovement = true,
                        GroupId = groupId,
                        Notes = $"Ekip Hareketi: {model.Notes}"
                    };

                    await CreateMovementAsync(movementModel);
                    successCount++;
                    _logger.LogInformation("Successfully created movement for {MemberName}", member.FullName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Team movement failed for member {MemberName} ({MemberId}): {Error}", member.FullName, member.Id, ex.Message);
                    errors.Add($"{member.FullName}: {ex.Message}");
                }
            }

            _logger.LogInformation("Team movement completed: {Success}/{Total} successful", successCount, teamMembers.Count);
            
            if (successCount == 0)
                throw new ApplicationException($"Hiçbir üye için hareket kaydı yapılamadı. Hatalar: {string.Join(", ", errors)}");

            if (errors.Any())
                _logger.LogWarning("Partial team movement success: {Success}/{Total}. Errors: {Errors}", 
                    successCount, teamMembers.Count, string.Join(", ", errors));

            return groupId;
        }
        catch (Exception ex) when (ex is not ApplicationException)
        {
            _logger.LogError(ex, "Error creating team movement");
            throw new ApplicationException("Ekip hareket kaydı yapılırken hata oluştu.");
        }
    }

    private async Task<List<VolunteerViewModel>> GetTeamMembersAsync(Guid teamId)
    {
        try
        {
            // Use large pagination to get ALL volunteers, then filter by TeamId
            var request = new PaginationRequest(0, 10000); // Increased limit
            var volunteers = await _volunteerService.GetVolunteersAsync(request);
            var teamMembers = volunteers.Items.Where(v => v.TeamId == teamId).ToList();
            
            _logger.LogInformation("GetTeamMembersAsync: Found {TeamMemberCount} members for team {TeamId} out of {TotalVolunteers} total volunteers", 
                teamMembers.Count, teamId, volunteers.Items.Count());
                
            return teamMembers;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team members for {TeamId}", teamId);
            return new List<VolunteerViewModel>();
        }
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
            var volunteers = await _volunteerService.GetVolunteersAsync(new PaginationRequest(0, 1000, qrId));
            var volunteer = volunteers.Items.FirstOrDefault(v => v.QRId == qrId);
            
            if (volunteer == null)
                return false;
                
            var sectors = await _sectorService.GetSectorsAsync();
            var hubSector = sectors.FirstOrDefault(s => s.Code == "BoO");
            
            if (hubSector == null)
                return false;
                
            // STATE MACHINE: NotEntered → InHub (null → BoO)
            var movementModel = new MovementCreateViewModel
            {
                VolunteerId = volunteer.Id,
                FromSectorId = null, // NotEntered state - no source sector
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
