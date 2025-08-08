using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record GetVolunteersQuery(PaginationRequest? PaginationRequest = null, string? Search = null) : IRequest<PaginationResponse<VolunteerResponse>>;

public sealed class GetVolunteersQueryHandler(IVolunteerRepository repository) 
    : IRequestHandler<GetVolunteersQuery, PaginationResponse<VolunteerResponse>>
{
    public async Task<PaginationResponse<VolunteerResponse>> Handle(GetVolunteersQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (volunteers, totalCount) = await repository.GetPaginatedAsync(paginationRequest, request.Search, cancellationToken);
        
        var responseList = VolunteerResponse.FromDomainList(volunteers);

        return new PaginationResponse<VolunteerResponse>(responseList, totalCount);
    }
}