using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Repositories;

public interface IVolunteerRepository
{
    Task<Volunteer?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Volunteer>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Volunteer>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken);
    Task<(List<Volunteer> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default);
    Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}