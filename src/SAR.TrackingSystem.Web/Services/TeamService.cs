using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using System.Net;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<TeamService> _logger;

    public TeamService(IHttpClientFactory httpClientFactory, ILogger<TeamService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("SarApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<List<TeamViewModel>> GetTeamsAsync()
    {
        var response = await _httpClient.GetAsync("/teams");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<TeamViewModel>>(json, _jsonOptions)!;
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

    public async Task<TeamDetailsViewModel?> GetTeamDetailsAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/teams/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TeamDetailsViewModel>(json, _jsonOptions);
    }

    public async Task<PaginatedResponse<TeamMemberViewModel>> GetTeamMembersAsync(Guid teamId, PaginationRequest request)
    {
        var response = await _httpClient.GetAsync($"/teams/{teamId}/members?page={request.Page}&pageSize={request.PageSize}");
        response.EnsureSuccessStatusCode();
        
        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<PaginationResponse<TeamMemberViewModel>>(json, _jsonOptions)!;
        
        return new PaginatedResponse<TeamMemberViewModel>
        {
            Items = apiResponse.Items,
            TotalCount = apiResponse.TotalCount,
            Page = request.Page + 1, // Convert from 0-based to 1-based
            PageSize = request.PageSize
        };
    }

    public async Task<List<TeamMemberViewModel>> GetTeamMembersListAsync(Guid teamId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<PaginatedResponse<TeamMemberViewModel>>($"/teams/{teamId}/members?page=1&pageSize=1000");
            return response?.Items?.ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team members list for team {TeamId}", teamId);
            return [];
        }
    }

    public async Task<bool> CreateTeamAsync(TeamViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/teams", model);
        return response.IsSuccessStatusCode;
    }
}
