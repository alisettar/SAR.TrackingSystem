namespace SAR.TrackingSystem.Application.Data.Volunteers.Commands;

public sealed record VolunteerRequest(
    string FullName,
    Guid TeamId,
    string Role,
    string QRId,
    Guid Id = default);
