using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record VolunteerResponse(
    Guid Id,
    string FullName,
    Guid TeamId,
    string TeamName,
    string? QRId,
    string? Role,
    VolunteerState CurrentState)
{
    public static VolunteerResponse FromDomain(Volunteer volunteer)
    {
        return new VolunteerResponse(
            volunteer.Id,
            volunteer.FullName,
            volunteer.TeamId,
            volunteer.Team.Name,
            volunteer.QRId,
            volunteer.Role,
            volunteer.CurrentState);
    }

    public static List<VolunteerResponse> FromDomainList(IEnumerable<Volunteer> volunteers)
    {
        return [.. volunteers.Select(FromDomain)];
    }
}
