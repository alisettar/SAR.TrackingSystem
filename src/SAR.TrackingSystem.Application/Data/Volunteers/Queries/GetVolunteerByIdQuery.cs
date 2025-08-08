using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetVolunteerByIdQuery(Guid Id) : IRequest<VolunteerResponse?>;

public sealed class GetVolunteerByIdQueryHandler(IVolunteerRepository repository) 
    : IRequestHandler<GetVolunteerByIdQuery, VolunteerResponse?>
{
    public async Task<VolunteerResponse?> Handle(GetVolunteerByIdQuery request, CancellationToken cancellationToken)
    {
        var volunteer = await repository.GetByIdAsync(request.Id, cancellationToken);
        
        return volunteer == null ? null : VolunteerResponse.FromDomain(volunteer);
    }
}