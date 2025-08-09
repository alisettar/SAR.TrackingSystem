using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface IDashboardService
{
    Task<DashboardStats> GetDashboardStatsAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IVolunteerService _volunteerService;
    private readonly IMovementService _movementService;

    public DashboardService(IVolunteerService volunteerService, IMovementService movementService)
    {
        _volunteerService = volunteerService;
        _movementService = movementService;
    }

    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            var volunteers = await _volunteerService.GetVolunteersAsync(1, 1000);
            var movements = await _movementService.GetMovementsAsync(1, 100);
            
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
