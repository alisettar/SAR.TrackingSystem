using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Data.Dashboard.Queries;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Repositories;

public class VolunteerRepository(SarDbContext context) : IVolunteerRepository
{
    public async Task<Volunteer?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

    public async Task<Volunteer?> GetByQRIdAsync(string qRId, CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .FirstOrDefaultAsync(v => v.QRId == qRId, cancellationToken);

    public async Task<bool> ExistsByQRIdAsync(string qrId, Guid? excludeVolunteerId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Volunteers.Where(v => v.QRId == qrId);

        if (excludeVolunteerId.HasValue)
            query = query.Where(v => v.Id != excludeVolunteerId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<Volunteer>> GetAllAsync(CancellationToken cancellationToken)
        => await context.Volunteers
            .Include(v => v.Team)
            .OrderBy(v => v.FullName)
            .ToListAsync(cancellationToken);

    public async Task<List<Volunteer>> GetVolunteersBySectorAsync(Guid sectorId, CancellationToken cancellationToken)
    {
        // Get volunteers currently in the specified sector (last movement ToSectorId = sectorId)
        var volunteersInSector = await (
            from v in context.Volunteers.AsNoTracking()
            where v.CurrentState == VolunteerState.InSector
            join m in (
                from movement in context.Movements.AsNoTracking()
                group movement by movement.VolunteerId into g
                select new { VolunteerId = g.Key, LastMovementTime = g.Max(x => x.MovementTime) }
            ) on v.Id equals m.VolunteerId
            join lastM in context.Movements.AsNoTracking() 
                on new { m.VolunteerId, m.LastMovementTime } 
                equals new { lastM.VolunteerId, LastMovementTime = lastM.MovementTime }
            where lastM.ToSectorId == sectorId
            join t in context.Teams.AsNoTracking() on v.TeamId equals t.Id
            select new { v, t, lastM.MovementTime }
        ).ToListAsync(cancellationToken);

        return volunteersInSector
            .Select(x => { x.v.Team = x.t; return x.v; })
            .OrderBy(v => v.Team.Name)
            .ThenBy(v => v.FullName)
            .ToList();
    }

    public async Task<(List<Volunteer> items, long totalCount)> GetByTeamIdAsync(Guid teamId, PaginationRequest request, CancellationToken cancellationToken = default)
    {
        var query = context.Volunteers.AsNoTracking().Include(v => v.Team).AsQueryable();

        // Filter by team ID
        query = query.Where(v => v.TeamId == teamId);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            // TODO: Türkçe karakter küçük büyük harf aramalarında sorun var. Düzeltilecek.
            query = query.Where(v => v.FullName.Contains(request.SearchText) ||
                                   (v.QRId != null && v.QRId.Contains(request.SearchText)) ||
                                   v.Team.Name.Contains(request.SearchText) ||
                                   v.Role.Contains(request.SearchText));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply ordering
        query = request.OrderBy switch
        {
            "FullName" => request.OrderDescending ? query.OrderByDescending(v => v.FullName) : query.OrderBy(v => v.FullName),
            "TeamName" => request.OrderDescending ? query.OrderByDescending(v => v.Team.Name) : query.OrderBy(v => v.Team.Name),
            "QRId" => request.OrderDescending ? query.OrderByDescending(v => v.QRId) : query.OrderBy(v => v.QRId),
            _ => request.OrderDescending ? query.OrderByDescending(v => v.FullName) : query.OrderBy(v => v.FullName)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Volunteer> items, long totalCount)> GetPaginatedAsync(PaginationRequest request, VolunteerState? stateFilter = null, CancellationToken cancellationToken = default)
    {
        var query = context.Volunteers.AsNoTracking().Include(v => v.Team).AsQueryable();

        // Apply state filter
        if (stateFilter.HasValue)
        {
            query = query.Where(v => v.CurrentState == stateFilter.Value);
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            // TODO: Türkçe karakter küçük büyük harf aramalarında sorun var. Düzeltilecek.
            query = query.Where(v => v.FullName.Contains(request.SearchText) ||
                                   (v.QRId != null && v.QRId.Contains(request.SearchText)) ||
                                   v.Team.Name.Contains(request.SearchText) ||
                                   v.Role.Contains(request.SearchText));
        }

        var totalCount = await query.LongCountAsync(cancellationToken);

        // Apply ordering
        query = request.OrderBy switch
        {
            "FullName" => request.OrderDescending ? query.OrderByDescending(v => v.FullName) : query.OrderBy(v => v.FullName),
            "TeamName" => request.OrderDescending ? query.OrderByDescending(v => v.Team.Name) : query.OrderBy(v => v.Team.Name),
            "QRId" => request.OrderDescending ? query.OrderByDescending(v => v.QRId) : query.OrderBy(v => v.QRId),
            _ => request.OrderDescending ? query.OrderByDescending(v => v.FullName) : query.OrderBy(v => v.FullName)
        };

        var items = await query
            .Skip(request.Page * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<VolunteerStateCounts> GetVolunteerStateCountsAsync(CancellationToken cancellationToken)
    {
        var totalCount = await context.Volunteers.AsNoTracking().LongCountAsync(cancellationToken);

        var stateCounts = await context.Volunteers
            .AsNoTracking()
            .GroupBy(v => v.CurrentState)
            .Select(g => new { State = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var nonArrivedCount = stateCounts.FirstOrDefault(x => x.State == Domain.Enums.VolunteerState.NotEntered)?.Count ?? 0;
        var inHubCount = stateCounts.FirstOrDefault(x => x.State == Domain.Enums.VolunteerState.InHub)?.Count ?? 0;
        var inSectorCount = stateCounts.FirstOrDefault(x => x.State == Domain.Enums.VolunteerState.InSector)?.Count ?? 0;
        var exitCount = stateCounts.FirstOrDefault(x => x.State == Domain.Enums.VolunteerState.Exited)?.Count ?? 0;

        return new VolunteerStateCounts(
            TotalVolunteers: totalCount,
            NonArrivedCount: nonArrivedCount,
            InHubCount: inHubCount,
            InSectorCount: inSectorCount,
            ExitCount: exitCount);
    }

    public async Task<List<SectorDistributionItem>> GetVolunteerSectorDistributionAsync(CancellationToken cancellationToken)
    {
        // Her volunteer'in son movement'ından ToSectorId'yi al
        var lastMovements = await (
            from v in context.Volunteers.AsNoTracking()
            join m in (
                from movement in context.Movements.AsNoTracking()
                group movement by movement.VolunteerId into g
                select new { VolunteerId = g.Key, LastMovementTime = g.Max(x => x.MovementTime) }
            ) on v.Id equals m.VolunteerId
            join lastM in context.Movements.AsNoTracking() on new { m.VolunteerId, m.LastMovementTime } equals new { lastM.VolunteerId, LastMovementTime = lastM.MovementTime }
            where lastM.ToSectorId != null // null (Exit) olanları ignore et
            join s in context.Sectors.AsNoTracking() on lastM.ToSectorId equals s.Id
            group s by new { s.Code, s.Name } into g
            select new SectorDistributionItem(g.Key.Code, g.Key.Name, g.Count())
        ).ToListAsync(cancellationToken);

        return lastMovements.Where(d => d.Count > 0).OrderByDescending(d => d.Count).ToList();
    }

    public async Task<List<CityDistributionItem>> GetVolunteerCityDistributionAsync(CancellationToken cancellationToken)
    {
        var cityDistribution = await (
            from v in context.Volunteers.AsNoTracking()
            where v.CurrentState != Domain.Enums.VolunteerState.NotEntered // Sadece gelenler
            join t in context.Teams.AsNoTracking() on v.TeamId equals t.Id
            where !string.IsNullOrEmpty(t.City)
            group t by t.City into g
            select new CityDistributionItem(g.Key, g.Count())
        ).ToListAsync(cancellationToken);

        return [.. cityDistribution.OrderByDescending(d => d.Count)];
    }

    public async Task<List<TeamVolunteerCount>> GetTeamVolunteerCountsAsync(CancellationToken cancellationToken)
    {
        var teamCounts = await (
            from t in context.Teams.AsNoTracking()
            select new TeamVolunteerCount(
                t.Name,
                t.City,
                context.Volunteers.AsNoTracking().Count(v => v.TeamId == t.Id && v.CurrentState != Domain.Enums.VolunteerState.NotEntered),
                context.Volunteers.AsNoTracking().Count(v => v.TeamId == t.Id)
            )
        ).ToListAsync(cancellationToken);

        return teamCounts.OrderBy(tc => tc.TeamName).ToList();
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


    public async Task<List<Guid>> GetTeamIdsWithNonArrivedVolunteersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Volunteers
            .Where(v => context.Movements.Any(m => m.VolunteerId == v.Id))
            .Select(v => v.TeamId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetTeamId(Guid teamId, CancellationToken cancellationToken = default)
    {
        var result = await context.Volunteers
            .Where(v => v.CurrentState == VolunteerState.InHub || v.CurrentState == VolunteerState.InSector)
            .Select(v => new
            {
                v.Id,
                v.TeamId,
                LatestMovement = context.Movements
                    .Where(m => m.VolunteerId == v.Id)
                    .OrderByDescending(m => m.MovementTime)
                    .FirstOrDefault()
            })
            .Where(x => x.LatestMovement != null)
            .Where(x => x.TeamId == teamId)
            .Select(x => x.TeamId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return result;
    }
}
