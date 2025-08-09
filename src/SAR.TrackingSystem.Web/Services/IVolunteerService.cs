using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface IVolunteerService
{
    Task<PaginatedResponse<VolunteerViewModel>> GetVolunteersAsync(int page = 1, int pageSize = 10, string search = "");
    Task<VolunteerViewModel?> GetVolunteerByIdAsync(Guid id);
    Task<Guid> CreateVolunteerAsync(VolunteerCreateViewModel model);
    Task<bool> UpdateVolunteerAsync(Guid id, VolunteerUpdateViewModel model);
    Task<bool> DeleteVolunteerAsync(Guid id);
}
