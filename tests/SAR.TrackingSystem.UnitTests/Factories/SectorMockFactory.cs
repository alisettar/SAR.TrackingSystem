using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class SectorMockFactory
{
    public static List<Sector> GetSampleSectors()
    {
        return
        [
            new Sector("ALAN_DIŞI", "Alan Dışı", true, false, true, true),
            new Sector("BOO", "BOO - Baz Operasyon Merkezi", true, true, false, true),
            new Sector("E-1", "E-1 Sektörü", true, false, false, false),
            new Sector("E-2", "E-2 Sektörü", true, false, false, false),
            new Sector("E2-A", "E2-A Alt Sektörü", true, false, false, false),
            new Sector("DIŞ", "Dış Sektör", true, false, false, false),
            new Sector("ÇIKIŞ", "Çıkış Noktası", true, false, true, true)
        ];
    }

    public static Sector GetEntrySection() => 
        new("ALAN_DIŞI", "Alan Dışı", true, false, true, true);

    public static Sector GetHubSection() => 
        new("BOO", "BOO - Baz Operasyon Merkezi", true, true, false, true);

    public static Sector GetExitSection() => 
        new("ÇIKIŞ", "Çıkış Noktası", true, false, true, true);

    public static Sector GetRegularSection() => 
        new("E-1", "E-1 Sektörü", true, false, false, false);
}
