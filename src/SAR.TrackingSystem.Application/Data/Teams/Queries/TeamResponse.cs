using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Application.Data.Teams.Queries;

public sealed record TeamResponse(
    Guid Id,
    string Name,
    string Code,
    int Capacity)
{
    public static TeamResponse FromDomain(Team team)
    {
        return new TeamResponse(
            team.Id,
            team.Name,
            team.Code,
            team.Capacity);
    }

    public static List<TeamResponse> FromDomainList(IEnumerable<Team> teams)
    {
        return [.. teams.Select(FromDomain)];
    }
}