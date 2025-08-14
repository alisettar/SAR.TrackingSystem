using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record GetDashboardStatsQuery() : IRequest<DashboardStatsResponse>;

public sealed class GetDashboardStatsQueryHandler(
    IVolunteerRepository volunteerRepository,
    ISectorRepository sectorRepository) 
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponse>
{
    public async Task<DashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await volunteerRepository.GetVolunteerStateCountsAsync(cancellationToken);
        var sectors = await sectorRepository.GetAllAsync(cancellationToken);
        
        var entryCount = stats.InHubCount + stats.InSectorCount;
        
        // Calculate sector rescue stats
        var totalExpectedVictims = sectors.Sum(s => s.ExpectedVictimCount);
        var totalRescued = sectors.Sum(s => s.RescuedCount);
        var totalExtricated = sectors.Sum(s => s.ExtricatedCount);
        
        // Map sectors for map display (exclude BoO and system sectors)
        var sectorMapData = sectors
            .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Coordinates) && s.Code != "BoO")
            .Select(s => new SectorMapResponse(
                s.Code,
                s.Name,
                s.Coordinates,
                s.RescuedCount,
                s.ExtricatedCount,
                s.ExpectedVictimCount,
                s.WorkAreaName
            ))
            .ToList();
        
        return new DashboardStatsResponse(
            TotalVolunteers: stats.TotalVolunteers,
            NonArrivedCount: stats.NonArrivedCount,
            InHubCount: stats.InHubCount,
            InSectorCount: stats.InSectorCount,
            EntryCount: entryCount,
            ExitCount: stats.ExitCount,
            TotalExpectedVictims: totalExpectedVictims,
            TotalRescuedCount: totalRescued,
            TotalExtricatedCount: totalExtricated,
            Sectors: sectorMapData);
    }
}
