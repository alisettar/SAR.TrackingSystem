using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface ISarApiService
{
    // Dashboard
    Task<DashboardStats> GetDashboardStatsAsync();
    
    // Volunteers
    Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(int page = 1, int pageSize = 10);
    Task<VolunteerViewModel?> GetVolunteerByIdAsync(Guid id);
    Task<Guid> CreateVolunteerAsync(VolunteerCreateViewModel model);
    Task<bool> UpdateVolunteerAsync(Guid id, VolunteerUpdateViewModel model);
    Task<bool> DeleteVolunteerAsync(Guid id);


    // Teams & Sectors (Dropdowns)
    Task<List<TeamViewModel>> GetTeamsAsync();
    Task<List<SectorViewModel>> GetSectorsAsync();
    Task<SectorViewModel?> GetSectorByIdAsync(Guid id);

    Task<TeamViewModel?> GetTeamByIdAsync(Guid id);

    Task<bool> CreateSectorAsync(SectorViewModel model);
    Task<bool> CreateTeamAsync(TeamViewModel model);

    // Movements
    Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(int page = 1, int pageSize = 10);
    Task<Guid> CreateMovementAsync(MovementCreateViewModel model);
}
