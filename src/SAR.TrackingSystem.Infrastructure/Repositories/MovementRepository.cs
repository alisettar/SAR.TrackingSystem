using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Repositories;

public class MovementRepository(SarDbContext context) : IMovementRepository
{
    public async Task<Movement?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Movements
            .Include(m => m.Volunteer)
            .Include(m => m.FromSector)
            .Include(m => m.ToSector)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<List<Movement>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Movements
            .Include(m => m.Volunteer)
            .Include(m => m.FromSector)
            .Include(m => m.ToSector)
            .OrderByDescending(m => m.MovementTime)
            .ToListAsync(cancellationToken);

    public async Task<(List<Movement> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var query = context.Movements
            .Include(m => m.Volunteer)
            .Include(m => m.FromSector)
            .Include(m => m.ToSector)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(m => m.Volunteer.FullName.Contains(request.SearchText) ||
                                   m.ToSector.Name.Contains(request.SearchText));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply ordering
        query = request.OrderBy switch
        {
            "VolunteerName" => request.OrderDescending ? query.OrderByDescending(m => m.Volunteer.FullName) : query.OrderBy(m => m.Volunteer.FullName),
            "ToSector" => request.OrderDescending ? query.OrderByDescending(m => m.ToSector.Name) : query.OrderBy(m => m.ToSector.Name),
            _ => request.OrderDescending ? query.OrderByDescending(m => m.MovementTime) : query.OrderBy(m => m.MovementTime)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Movement movement, CancellationToken cancellationToken)
    {
        context.Movements.Add(movement);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasMovementsAsync(Guid volunteerId, CancellationToken cancellationToken)
        => await context.Movements.AnyAsync(m => m.VolunteerId == volunteerId, cancellationToken);
}