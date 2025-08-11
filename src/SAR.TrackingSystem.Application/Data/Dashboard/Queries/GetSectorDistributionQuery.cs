using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record GetSectorDistributionQuery() : IRequest<SectorDistributionResponse>;

public sealed class GetSectorDistributionQueryHandler(
    IVolunteerRepository volunteerRepository) 
    : IRequestHandler<GetSectorDistributionQuery, SectorDistributionResponse>
{
    public async Task<SectorDistributionResponse> Handle(GetSectorDistributionQuery request, CancellationToken cancellationToken)
    {
        var distribution = await volunteerRepository.GetVolunteerSectorDistributionAsync(cancellationToken);
        
        return new SectorDistributionResponse(distribution);
    }
}
