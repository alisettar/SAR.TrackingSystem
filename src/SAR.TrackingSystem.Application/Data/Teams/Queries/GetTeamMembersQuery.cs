using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Teams.Queries;

public sealed record GetTeamMembersQuery(Guid TeamId, PaginationRequest Pagination) : IRequest<PaginationResponse<TeamMemberResponse>>;

public sealed class GetTeamMembersQueryHandler(IVolunteerRepository volunteerRepository)
    : IRequestHandler<GetTeamMembersQuery, PaginationResponse<TeamMemberResponse>>
{
    public async Task<PaginationResponse<TeamMemberResponse>> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken)
    {
        var volunteers = await volunteerRepository.GetByTeamIdAsync(request.TeamId, cancellationToken);

        // Apply pagination
        var totalCount = volunteers.Count;
        var skip = (request.Pagination.Page - 1) * request.Pagination.PageSize;
        var pagedVolunteers = volunteers.Skip(skip).Take(request.Pagination.PageSize).ToList();

        var members = pagedVolunteers.Select(v => new TeamMemberResponse
        {
            Id = v.Id,
            FullName = v.FullName,
            QRId = v.QRId,
            Role = v.Role
        }).ToList();

        return new PaginationResponse<TeamMemberResponse>(members, totalCount);
    }
}
