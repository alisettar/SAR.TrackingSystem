using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record GetDashboardStatsQuery() : IRequest<DashboardStatsResponse>;

public sealed class GetDashboardStatsQueryHandler(IVolunteerRepository volunteerRepository) 
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsResponse>
{
    public async Task<DashboardStatsResponse> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var stats = await volunteerRepository.GetVolunteerStateCountsAsync(cancellationToken);
        
        var entryCount = stats.InHubCount + stats.InSectorCount;
        
        return new DashboardStatsResponse(
            TotalVolunteers: stats.TotalVolunteers,
            NonArrivedCount: stats.NonArrivedCount,
            InHubCount: stats.InHubCount,
            InSectorCount: stats.InSectorCount,
            EntryCount: entryCount,
            ExitCount: stats.ExitCount);
    }
}
