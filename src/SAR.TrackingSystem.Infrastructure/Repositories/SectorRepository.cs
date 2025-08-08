using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Repositories;

public class SectorRepository(SarDbContext context) : ISectorRepository
{
    public async Task<Sector?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Sectors.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<List<Sector>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Sectors
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
}