using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Teams.Queries;

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