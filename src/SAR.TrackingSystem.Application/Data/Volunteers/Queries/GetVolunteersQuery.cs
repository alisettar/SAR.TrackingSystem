using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetVolunteersQuery(PaginationRequest? PaginationRequest = null, VolunteerState? StateFilter = null) : IRequest<PaginationResponse<VolunteerResponse>>;

public sealed class GetVolunteersQueryHandler(IVolunteerRepository repository) 
    : IRequestHandler<GetVolunteersQuery, PaginationResponse<VolunteerResponse>>
{
    public async Task<PaginationResponse<VolunteerResponse>> Handle(GetVolunteersQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (volunteers, totalCount) = await repository.GetPaginatedAsync(paginationRequest, request.StateFilter, cancellationToken);
        
        var responseList = VolunteerResponse.FromDomainList(volunteers);

        return new PaginationResponse<VolunteerResponse>(responseList, totalCount);
    }
}