using SAR.TrackingSystem.Web.Models;

namespace SAR.TrackingSystem.Web.Services.Interfaces;

public interface ISectorService
{
    Task<List<SectorViewModel>> GetSectorsAsync();
    Task<SectorViewModel?> GetSectorByIdAsync(Guid id);
    Task<SectorStatisticsViewModel?> GetSectorStatisticsAsync(Guid id);
    Task<bool> CreateSectorAsync(SectorViewModel model);
    Task<bool> DeleteSectorAsync(Guid id);
}
