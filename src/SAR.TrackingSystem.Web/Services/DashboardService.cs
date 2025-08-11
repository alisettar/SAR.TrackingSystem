using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;
using SAR.TrackingSystem.Web.Services.Interfaces;
using System.Text.Json;

namespace SAR.TrackingSystem.Web.Services;

public class DashboardService(
    HttpClient httpClient,
    IVolunteerService volunteerService) : IDashboardService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/dashboard/stats");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiStats = JsonSerializer.Deserialize<ApiDashboardStats>(content, _jsonOptions);

            return new DashboardStats
            {
                TotalVolunteers = apiStats?.TotalVolunteers ?? 0,
                NonArrivedCount = apiStats?.NonArrivedCount ?? 0,
                InHubCount = apiStats?.InHubCount ?? 0,
                InSectorCount = apiStats?.InSectorCount ?? 0,
                EntryCount = apiStats?.EntryCount ?? 0,
                ExitCount = apiStats?.ExitCount ?? 0
            };
        }
        catch (Exception)
        {
            return new DashboardStats { TotalVolunteers = 0 };
        }
    }

    public async Task<TeamDistributionData> GetTeamDistributionAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/dashboard/team-distribution");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiData = JsonSerializer.Deserialize<ApiTeamDistribution>(content, _jsonOptions);

            return new TeamDistributionData
            {
                Teams = apiData?.Items.Select(x => new TeamInfo
                {
                    Name = x.TeamName,
                    City = x.City,
                    ArrivedCount = x.ArrivedCount,
                    TotalCount = x.TotalCount,
                    Percentage = x.TotalCount > 0 ? (int)Math.Round((double)x.ArrivedCount / x.TotalCount * 100) : 0
                }).OrderBy(t => t.Name).ToList() ?? []
            };
        }
        catch (Exception)
        {
            return new TeamDistributionData();
        }
    }

    public async Task<SectorDistributionData> GetSectorDistributionAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/dashboard/sector-distribution");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiData = JsonSerializer.Deserialize<ApiSectorDistribution>(content, _jsonOptions);

            return new SectorDistributionData
            {
                Labels = apiData?.Items.Select(x => x.SectorCode).ToList() ?? [],
                Data = apiData?.Items.Select(x => x.Count).ToList() ?? [],
                BackgroundColors = GenerateColors(apiData?.Items.Count ?? 0)
            };
        }
        catch (Exception)
        {
            return new SectorDistributionData();
        }
    }

    public async Task<CityDistributionData> GetCityDistributionAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/dashboard/city-distribution");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var apiData = JsonSerializer.Deserialize<ApiCityDistribution>(content, _jsonOptions);

            return new CityDistributionData
            {
                Labels = apiData?.Items.Select(x => x.CityName).ToList() ?? [],
                Data = apiData?.Items.Select(x => x.Count).ToList() ?? [],
                BackgroundColors = GenerateColors(apiData?.Items.Count ?? 0)
            };
        }
        catch (Exception)
        {
            return new CityDistributionData();
        }
    }

    public async Task<List<VolunteerViewModel>> GetNonArrivedVolunteersAsync()
    {
        try
        {
            var volunteers = await volunteerService.GetVolunteersAsync(new PaginationRequest(0, 1000));
            return [.. volunteers.Items.Where(v => v.CurrentState == 0)];
        }
        catch (Exception)
        {
            return [];
        }
    }

    private static List<string> GenerateColors(int count)
    {
        var colors = new[]
        {
            "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0",
            "#9966FF", "#FF9F40", "#FF6384", "#C9CBCF"
        };
        return [.. colors.Take(count)];
    }
}

// API response model
public record ApiDashboardStats(
    long TotalVolunteers,
    int NonArrivedCount,
    int InHubCount,
    int InSectorCount,
    int EntryCount,
    int ExitCount);

// Sector distribution models
public record ApiSectorDistribution(List<ApiSectorDistributionItem> Items);
public record ApiSectorDistributionItem(string SectorCode, string SectorName, int Count);

public class SectorDistributionData
{
    public List<string> Labels { get; set; } = [];
    public List<int> Data { get; set; } = [];
    public List<string> BackgroundColors { get; set; } = [];
}

// City distribution models
public record ApiCityDistribution(List<ApiCityDistributionItem> Items);
public record ApiCityDistributionItem(string CityName, int Count);

public class CityDistributionData
{
    public List<string> Labels { get; set; } = [];
    public List<int> Data { get; set; } = [];
    public List<string> BackgroundColors { get; set; } = [];
}

// Team distribution models
public record ApiTeamDistribution(List<ApiTeamDistributionItem> Items);
public record ApiTeamDistributionItem(string TeamName, string City, int ArrivedCount, int TotalCount);

public class TeamDistributionData
{
    public List<TeamInfo> Teams { get; set; } = [];
}

public class TeamInfo
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ArrivedCount { get; set; }
    public int TotalCount { get; set; }
    public int Percentage { get; set; }
}
