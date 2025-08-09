using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;

namespace SAR.TrackingSystem.Web.Services;

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
}

public class DashboardService(
    IVolunteerService volunteerService) : IDashboardService
{
    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            var volunteers = await volunteerService.GetVolunteersAsync(new PaginationRequest(0, 1000));
            
            return new DashboardStats
            {
                TotalVolunteers = volunteers.TotalCount,
                InHubCount = 45,
                InSectorCount = 25,
                EntryCount = 15,
                ExitCount = 5
            };
        }
        catch (Exception)
        {
            return new DashboardStats { TotalVolunteers = 0 };
        }
    }
}
