using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface IMovementService
{
    Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<Guid> CreateMovementAsync(MovementCreateViewModel model);
    Task<Guid> CreateTeamMovementAsync(TeamMovementCreateViewModel model);
    Task<bool> DeleteMovementAsync(Guid id);
    Task<bool> CreateQuickEntryAsync(string qrId);
    Task<List<MovementViewModel>> GetRecentMovementsAsync(int count = 5);
}
