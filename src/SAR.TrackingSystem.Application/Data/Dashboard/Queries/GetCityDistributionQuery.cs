using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Dashboard.Queries;

public sealed record GetCityDistributionQuery() : IRequest<CityDistributionResponse>;

public sealed class GetCityDistributionQueryHandler(IVolunteerRepository volunteerRepository) 
    : IRequestHandler<GetCityDistributionQuery, CityDistributionResponse>
{
    public async Task<CityDistributionResponse> Handle(GetCityDistributionQuery request, CancellationToken cancellationToken)
    {
        var distribution = await volunteerRepository.GetVolunteerCityDistributionAsync(cancellationToken);
        
        return new CityDistributionResponse(distribution);
    }
}
