using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class SectorMockFactory
{
    public static List<Sector> GetSampleSectors()
    {
        return
        [
            new Sector("Entry", "Giriş", true, false, true, true),
            new Sector("BoO", "BoO - Baz Operasyon Merkezi", true, true, false, true),
            new Sector("E-1", "E-1 Sektörü", true, false, false, false),
            new Sector("E-2", "E-2 Sektörü", true, false, false, false),
            new Sector("E2-A", "E2-A Alt Sektörü", true, false, false, false),
            new Sector("DIŞ", "Dış Sektör", true, false, false, false),
            new Sector("Exit", "Çıkış", true, false, true, true)
        ];
    }

    public static Sector GetEntrySection() => 
        new("Entry", "Alan Dışı", true, false, true, true);

    public static Sector GetHubSection() => 
        new("BoO", "BoO - Baz Operasyon Merkezi", true, true, false, true);

    public static Sector GetExitSection() => 
        new("Exit", "Çıkış", true, false, true, true);

    public static Sector GetRegularSection() => 
        new("E-1", "E-1 Sektörü", true, false, false, false);
}
