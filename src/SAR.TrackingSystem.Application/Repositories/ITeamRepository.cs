using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Repositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Team>> GetAllAsync(CancellationToken cancellationToken);
    Task<Team?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task AddAsync(Team team, CancellationToken cancellationToken);
}