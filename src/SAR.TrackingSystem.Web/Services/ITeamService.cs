using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface ITeamService
{
    Task<List<TeamViewModel>> GetTeamsAsync();
    Task<TeamViewModel?> GetTeamByIdAsync(Guid id);
    Task<TeamDetailsViewModel?> GetTeamDetailsAsync(Guid id);
    Task<PaginatedResponse<TeamMemberViewModel>> GetTeamMembersAsync(Guid teamId, int page = 1, int pageSize = 10);
    Task<List<TeamMemberViewModel>> GetTeamMembersListAsync(Guid teamId);
    Task<bool> CreateTeamAsync(TeamViewModel model);
}
