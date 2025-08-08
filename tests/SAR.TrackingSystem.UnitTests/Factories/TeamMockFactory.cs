using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class TeamMockFactory
{
    public static List<Team> GetSampleTeams()
    {
        return
        [
            new Team("A", "A Tipi", 10, true),
            new Team("B", "B Tipi", 10, true),
            new Team("C", "C Tipi", 10, true),
            new Team("D", "D Tipi", 10, true),
            new Team("MEDIKAL", "Medikal Tim", 5, true),
            new Team("LOJISTIK", "Lojistik Tim", 8, true),
            new Team("YONETIM", "Yönetim", 3, true)
        ];
    }

    public static Team GetMedikalTeam()
    {
        return new Team("MEDIKAL", "Medikal Tim", 5, true);
    }

    public static Team GetATeam()
    {
        return new Team("A", "A Tipi", 10, true);
    }
}
