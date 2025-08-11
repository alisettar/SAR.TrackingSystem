using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public record GetTeamDistributionQuery : IRequest<TeamDistributionResponse>;

public class GetTeamDistributionQueryHandler(IVolunteerRepository volunteerRepository)
    : IRequestHandler<GetTeamDistributionQuery, TeamDistributionResponse>
{
    public async Task<TeamDistributionResponse> Handle(GetTeamDistributionQuery request, CancellationToken cancellationToken)
    {
        var teamCounts = await volunteerRepository.GetTeamVolunteerCountsAsync(cancellationToken);

        var items = teamCounts.Select(tc => new TeamDistributionItem(
            tc.TeamName,
            tc.City ?? "Belirtilmemiş",
            tc.ArrivedCount,
            tc.TotalCount))
            .OrderBy(x => x.TeamName)
            .ToList();

        return new TeamDistributionResponse(items);
    }
}
