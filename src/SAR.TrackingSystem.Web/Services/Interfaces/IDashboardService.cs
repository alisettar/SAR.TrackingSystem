using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
    Task<List<VolunteerViewModel>> GetNonArrivedVolunteersAsync();
    Task<SectorDistributionData> GetSectorDistributionAsync();
    Task<CityDistributionData> GetCityDistributionAsync();
    Task<TeamDistributionData> GetTeamDistributionAsync();
}
