using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Repositories;

public class VolunteerRepository(SarDbContext context) : IVolunteerRepository
{
    public async Task<Volunteer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<List<Volunteer>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .OrderBy(v => v.FullName)
            .ToListAsync(cancellationToken);

    public async Task<List<Volunteer>> GetByTeamIdAsync(Guid teamId, CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .Where(v => v.TeamId == teamId)
            .ToListAsync(cancellationToken);

    public async Task<(List<Volunteer> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, string? search = null, CancellationToken cancellationToken = default)
    {
        var query = context.Volunteers.Include(v => v.Team).AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(v => v.FullName.Contains(search) ||
                                   v.TcKimlik.ToString().Contains(search) ||
                                   v.Team.Name.Contains(search));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply ordering
        query = request.OrderBy switch
        {
            "FullName" => request.OrderDescending ? query.OrderByDescending(v => v.FullName) : query.OrderBy(v => v.FullName),
            "TeamName" => request.OrderDescending ? query.OrderByDescending(v => v.Team.Name) : query.OrderBy(v => v.Team.Name),
            _ => request.OrderDescending ? query.OrderByDescending(v => v.CreatedAt) : query.OrderBy(v => v.CreatedAt)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        context.Volunteers.Add(volunteer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Volunteer volunteer, CancellationToken cancellationToken)
    {
        context.Volunteers.Update(volunteer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var volunteer = await GetByIdAsync(id, cancellationToken);
        if (volunteer != null)
        {
            context.Volunteers.Remove(volunteer);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}