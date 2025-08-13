using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Dashboard.Queries;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Repositories;

public interface IVolunteerRepository
{
    Task<Volunteer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByQRIdAsync(string qrId, Guid? excludeVolunteerId = null, CancellationToken cancellationToken = default);
    Task<List<Volunteer>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Volunteer>> GetVolunteersBySectorAsync(Guid sectorId, CancellationToken cancellationToken);
    Task<(List<Volunteer> items, long totalCount)> GetByTeamIdAsync(Guid teamId, PaginationRequest request, CancellationToken cancellationToken);
    Task<(List<Volunteer> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, VolunteerState? stateFilter = null, CancellationToken cancellationToken = default);
    Task<VolunteerStateCounts> GetVolunteerStateCountsAsync(CancellationToken cancellationToken);
    Task<List<SectorDistributionItem>> GetVolunteerSectorDistributionAsync(CancellationToken cancellationToken);
    Task<List<CityDistributionItem>> GetVolunteerCityDistributionAsync(CancellationToken cancellationToken);
    Task<List<TeamVolunteerCount>> GetTeamVolunteerCountsAsync(CancellationToken cancellationToken);
    Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Volunteer?> GetByQRIdAsync(string qRId, CancellationToken cancellationToken);
}