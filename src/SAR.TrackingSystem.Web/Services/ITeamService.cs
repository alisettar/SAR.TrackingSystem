using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;

namespace SAR.TrackingSystem.Web.Services;

public interface ITeamService
{
    Task<List<TeamViewModel>> GetTeamsAsync();
    Task<TeamViewModel?> GetTeamByIdAsync(Guid id);
    Task<TeamDetailsViewModel?> GetTeamDetailsAsync(Guid id);
    Task<PaginatedResponse<TeamMemberViewModel>> GetTeamMembersAsync(Guid teamId, PaginationRequest request);
    Task<List<TeamMemberViewModel>> GetTeamMembersListAsync(Guid teamId);
    Task<bool> CreateTeamAsync(TeamViewModel model);
}
