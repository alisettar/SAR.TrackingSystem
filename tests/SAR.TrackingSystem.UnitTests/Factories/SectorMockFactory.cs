using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class SectorMockFactory
{
    public static List<Sector> GetSampleSectors()
    {
        return
        [
            new Sector("BoO", "Operasyon Merkezi (BoO)", true, true, false, true),
            new Sector("E-1", "E-1 Sektörü", true, false, false, false),
            new Sector("E-2", "E-2 Sektörü", true, false, false, false),
            new Sector("E2-A", "E2-A Alt Sektörü", true, false, false, false),
            new Sector("E2-B", "E2-B Alt Sektörü", true, false, false, false)
        ];
    }

    public static Sector GetHubSection() => 
        new("BoO", "Operasyon Merkezi (BoO)", true, true, false, true);

    public static Sector GetRegularSection() => 
        new("E-1", "E-1 Sektörü", true, false, false, false);
}
