using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetTeamIdsWithNonArrivedVolunteersQuery : IRequest<List<Guid>>;

public sealed class GetTeamIdsWithNonArrivedVolunteersQueryHandler(IVolunteerRepository repository)
    : IRequestHandler<GetTeamIdsWithNonArrivedVolunteersQuery, List<Guid>>
{
    public async Task<List<Guid>> Handle(GetTeamIdsWithNonArrivedVolunteersQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetTeamIdsWithNonArrivedVolunteersAsync(cancellationToken);
    }
}