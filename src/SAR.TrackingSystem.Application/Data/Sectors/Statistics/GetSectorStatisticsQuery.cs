using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Sectors.Statistics;

public sealed record GetSectorStatisticsQuery(Guid SectorId) : IRequest<SectorStatisticsResponse>;

public sealed class GetSectorStatisticsQueryHandler(
    IVolunteerRepository volunteerRepository) : IRequestHandler<GetSectorStatisticsQuery, SectorStatisticsResponse>
{
    public async Task<SectorStatisticsResponse> Handle(GetSectorStatisticsQuery request, CancellationToken cancellationToken)
    {
        var volunteers = await volunteerRepository.GetVolunteersBySectorAsync(request.SectorId, cancellationToken);

        var teams = volunteers
            .GroupBy(v => new { v.TeamId, v.Team.Name, v.Team.Code })
            .Select(g => new TeamInSectorResponse(
                g.Key.TeamId,
                g.Key.Name,
                g.Key.Code,
                g.Count(),
                [.. g.Select(v => new VolunteerInSectorResponse(
                    v.Id,
                    v.FullName,
                    v.Role,
                    v.QRId ?? "",
                    DateTime.UtcNow // We'll need to get actual last movement time
                ))]
            ))
            .OrderByDescending(t => t.MemberCount)
            .ToList();

        var roleDistribution = volunteers
            .GroupBy(v => v.Role)
            .Select(g => new RoleDistributionResponse(g.Key, g.Count()))
            .OrderByDescending(r => r.Count)
            .ToList();

        return new SectorStatisticsResponse(
            volunteers.Count,
            teams,
            roleDistribution
        );
    }
}
