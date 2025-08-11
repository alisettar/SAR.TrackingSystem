namespace SAR.TrackingSystem.Application.Repositories;

public sealed record VolunteerStateCounts(
    long TotalVolunteers,
    int NonArrivedCount,
    int InHubCount,
    int InSectorCount,
    int ExitCount);

public sealed record TeamVolunteerCount(
    string TeamName,
    string? City,
    int ArrivedCount,
    int TotalCount);
