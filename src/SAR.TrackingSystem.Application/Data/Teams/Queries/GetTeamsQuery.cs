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

public sealed record GetTeamByIdQuery(Guid Id) : IRequest<TeamResponse?>;

public sealed class GetTeamByIdQueryHandler(ITeamRepository repository)
    : IRequestHandler<GetTeamByIdQuery, TeamResponse?>
{
    public async Task<TeamResponse?> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await repository.GetByIdAsync(request.Id, cancellationToken);
        return team == null ? null : TeamResponse.FromDomain(team);
    }
}