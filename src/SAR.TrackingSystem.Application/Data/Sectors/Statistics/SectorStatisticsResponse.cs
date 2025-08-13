namespace SAR.TrackingSystem.Application.Data.Sectors.Statistics;

public sealed record SectorStatisticsResponse(
    int TotalVolunteers,
    List<TeamInSectorResponse> Teams,
    List<RoleDistributionResponse> RoleDistribution
);

public sealed record TeamInSectorResponse(
    Guid TeamId,
    string TeamName,
    string TeamCode,
    int MemberCount,
    List<VolunteerInSectorResponse> Members
);

public sealed record VolunteerInSectorResponse(
    Guid Id,
    string FullName,
    string Role,
    string QRId,
    DateTime LastMovementTime
);

public sealed record RoleDistributionResponse(
    string Role,
    int Count
);
