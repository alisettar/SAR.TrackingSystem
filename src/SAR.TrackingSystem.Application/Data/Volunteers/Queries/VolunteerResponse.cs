using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record VolunteerResponse(
    Guid Id,
    string FullName,
    Guid TeamId,
    string TeamName,
    string? QRId,
    string? Role)
{
    public static VolunteerResponse FromDomain(Volunteer volunteer)
    {
        return new VolunteerResponse(
            volunteer.Id,
            volunteer.FullName,
            volunteer.TeamId,
            volunteer.Team.Name,
            volunteer.QRId,
            volunteer.Role);
    }

    public static List<VolunteerResponse> FromDomainList(IEnumerable<Volunteer> volunteers)
    {
        return [.. volunteers.Select(FromDomain)];
    }
}
