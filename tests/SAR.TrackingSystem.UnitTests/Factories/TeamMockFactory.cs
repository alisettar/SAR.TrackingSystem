using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class TeamMockFactory
{
    public static List<Team> GetSampleTeams()
    {
        return
        [
            new Team("A", "A Timi", null, true),
            new Team("B", "B Timi", null, true),
            new Team("C", "C Timi", null, true),
            new Team("D", "D Timi", null, true),
            new Team("MEDIKAL", "Medikal Tim", null, true),
            new Team("LOJISTIK", "Lojistik Tim", null, true),
            new Team("YONETIM", "Yönetim", null, true)
        ];
    }

    public static Team GetMedikalTeam()
    {
        return new Team("MEDIKAL", "Medikal Tim", null, true);
    }

    public static Team GetATeam()
    {
        return new Team("A", "A Timi", null, true);
    }
}
