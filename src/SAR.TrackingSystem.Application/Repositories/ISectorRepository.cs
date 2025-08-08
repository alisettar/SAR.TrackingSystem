using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Repositories;

public interface ISectorRepository
{
    Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Sector>> GetAllAsync(CancellationToken cancellationToken);
    Task<Sector?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(Sector sector, CancellationToken cancellationToken);
}