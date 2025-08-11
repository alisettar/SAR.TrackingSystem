using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Teams.Queries;

public sealed record TeamResponse(
    Guid Id,
    string Name,
    string Code,
    string? City)
{
    public static TeamResponse FromDomain(Team team)
    {
        return new TeamResponse(
            team.Id,
            team.Name,
            team.Code,
            team.City);
    }

    public static List<TeamResponse> FromDomainList(IEnumerable<Team> teams)
    {
        return [.. teams.Select(FromDomain)];
    }
}

public sealed record TeamDetailsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? City { get; init; }
    public List<TeamMemberResponse> Members { get; init; } = [];
}

public sealed record TeamMemberResponse
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? QRId { get; init; }
    public string? Role { get; init; }
}
