using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;

namespace SAR.TrackingSystem.Web.Services.Interfaces;

public interface IMovementService
{
    Task<PaginatedResponse<MovementViewModel>> GetMovementsAsync(PaginationRequest request);
    Task<Guid> CreateMovementAsync(MovementCreateViewModel model);
    Task<Guid> CreateTeamMovementAsync(TeamMovementCreateViewModel model);
    Task<bool> DeleteMovementAsync(Guid id);
    Task<bool> CreateQuickEntryAsync(string qrId);
    Task<List<MovementViewModel>> GetRecentMovementsAsync(int count = 5);
}
