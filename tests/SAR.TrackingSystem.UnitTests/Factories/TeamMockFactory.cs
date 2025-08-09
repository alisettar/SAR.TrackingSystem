using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class TeamMockFactory
{
    public static List<Team> GetSampleTeams()
    {
        return
        [
            new Team("A", "A Tipi", null, true),
            new Team("B", "B Tipi", null, true),
            new Team("C", "C Tipi", null, true),
            new Team("D", "D Tipi", null, true),
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
        return new Team("A", "A Tipi", null, true);
    }
}
