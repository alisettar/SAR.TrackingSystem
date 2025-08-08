using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Queries;

public sealed record VolunteerResponse(
    Guid Id,
    long TcKimlik,
    string FullName,
    Guid TeamId,
    string TeamName,
    string BloodType,
    string Phone,
    string EmergencyContactName,
    string EmergencyContactPhone,
    string? Buddy1,
    string? Buddy2,
    bool IsActive,
    DateTime CreatedAt)
{
    public static VolunteerResponse FromDomain(Volunteer volunteer)
    {
        return new VolunteerResponse(
            volunteer.Id,
            volunteer.TcKimlik,
            volunteer.FullName,
            volunteer.TeamId,
            volunteer.Team.Name,
            volunteer.BloodType,
            volunteer.Phone,
            volunteer.EmergencyContactName,
            volunteer.EmergencyContactPhone,
            volunteer.Buddy1,
            volunteer.Buddy2,
            volunteer.IsActive,
            volunteer.CreatedAt);
    }

    public static List<VolunteerResponse> FromDomainList(IEnumerable<Volunteer> volunteers)
    {
        return [.. volunteers.Select(FromDomain)];
    }
}