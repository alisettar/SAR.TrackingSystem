namespace SAR.TrackingSystem.Application.Data.Volunteers.Commands;

public sealed record VolunteerRequest(
    long TcKimlik,
    string FullName,
    Guid TeamId,
    string BloodType,
    string Phone,
    string EmergencyContactName,
    string EmergencyContactPhone,
    string? Buddy1 = null,
    string? Buddy2 = null,
    bool IsActive = true,
    Guid Id = default);