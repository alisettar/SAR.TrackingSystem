using SAR.TrackingSystem.Web.Models;
using SAR.TrackingSystem.Web.Models.Common;

namespace SAR.TrackingSystem.Web.Services.Interfaces;

public interface IVolunteerService
{
    Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(PaginationRequest request, string? filter = null);
    Task<VolunteerViewModel?> GetVolunteerByIdAsync(Guid id);
    Task<VolunteerViewModel?> GetVolunteerByQRIdAsync(string qrid);
    Task<Guid> CreateVolunteerAsync(VolunteerCreateViewModel model);
    Task<bool> UpdateVolunteerAsync(Guid id, VolunteerUpdateViewModel model);
    Task<bool> DeleteVolunteerAsync(Guid id);
    Task<List<MovementHistoryViewModel>> GetVolunteerMovementHistoryAsync(Guid id);
}
