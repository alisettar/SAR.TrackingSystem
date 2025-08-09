using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services;

public interface ISectorService
{
    Task<List<SectorViewModel>> GetSectorsAsync();
    Task<SectorViewModel?> GetSectorByIdAsync(Guid id);
    Task<bool> CreateSectorAsync(SectorViewModel model);
    Task<bool> DeleteSectorAsync(Guid id);
}
