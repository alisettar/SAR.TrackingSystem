using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Dashboard.Queries;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Repositories;

public interface IVolunteerRepository
{
    Task<Volunteer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByQRIdAsync(string qrId, Guid? excludeVolunteerId = null, CancellationToken cancellationToken = default);
    Task<List<Volunteer>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Volunteer>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<(List<Volunteer> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default);
    Task<VolunteerStateCounts> GetVolunteerStateCountsAsync(CancellationToken cancellationToken);
    Task<List<SectorDistributionItem>> GetVolunteerSectorDistributionAsync(CancellationToken cancellationToken);
    Task<List<CityDistributionItem>> GetVolunteerCityDistributionAsync(CancellationToken cancellationToken);
    Task<List<TeamVolunteerCount>> GetTeamVolunteerCountsAsync(CancellationToken cancellationToken);
    Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}