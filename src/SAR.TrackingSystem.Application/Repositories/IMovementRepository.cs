using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Repositories;

public interface IMovementRepository
{
    Task<Movement?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Movement>> GetAllAsync(CancellationToken cancellationToken);
    Task<(List<Movement> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task AddAsync(Movement movement, CancellationToken cancellationToken);
    Task<bool> HasMovementsAsync(Guid volunteerId, CancellationToken cancellationToken);
    Task<Movement?> GetLastMovementAsync(Guid volunteerId, CancellationToken cancellationToken);
    Task<List<Movement>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}