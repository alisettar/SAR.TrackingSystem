using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Repositories;

public class TeamRepository(SarDbContext context) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Teams.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<Team?> GetByCodeAsync(string code, CancellationToken cancellationToken)
        => await context.Teams.FirstOrDefaultAsync(t => t.Code == code, cancellationToken);

    public async Task<List<Team>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Teams
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Team team, CancellationToken cancellationToken)
    {
        await context.Teams.AddAsync(team, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}