using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetVolunteerByQRIdQuery(string QRId) : IRequest<VolunteerResponse?>;

public sealed class GetVolunteerByQRIdQueryHandler(IVolunteerRepository repository) 
    : IRequestHandler<GetVolunteerByQRIdQuery, VolunteerResponse?>
{
    public async Task<VolunteerResponse?> Handle(GetVolunteerByQRIdQuery request, CancellationToken cancellationToken)
    {
        var volunteer = await repository.GetByQRIdAsync(request.QRId, cancellationToken);
        
        return volunteer == null ? null : VolunteerResponse.FromDomain(volunteer);
    }
}