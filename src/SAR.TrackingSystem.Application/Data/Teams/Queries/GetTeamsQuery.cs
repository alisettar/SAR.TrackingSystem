using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Teams.Queries;

public sealed record GetTeamsQuery : IRequest<List<TeamResponse>>;

public sealed class GetTeamsQueryHandler(ITeamRepository repository) 
    : IRequestHandler<GetTeamsQuery, List<TeamResponse>>
{
    public async Task<List<TeamResponse>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await repository.GetAllAsync(cancellationToken);
        return TeamResponse.FromDomainList(teams);
    }
}