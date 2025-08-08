using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Domain.SeedData;

public static class DefaultSeedData
{
    public static List<Sector> Sectors =>
    [
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000001"), Code = "ALAN_DIŞI", Name = "Alan Dışı", IsActive = false },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000002"), Code = "BOO", Name = "Base of Operations", IsEntryPoint = true, IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000003"), Code = "E-1", Name = "Sektör E-1", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000004"), Code = "E-2", Name = "Sektör E-2", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000005"), Code = "E2-A", Name = "Sektör E2-A", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000006"), Code = "E2-B", Name = "Sektör E2-B", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000007"), Code = "ÇIKIŞ", Name = "Çıkış", IsExitPoint = true, IsActive = true }
    ];

    public static List<Team> Teams =>
    [
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000001"), Name = "EKİP LİDERİ", Code = "LDR", Capacity = 1 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000002"), Name = "LİDER YARDIMCISI", Code = "ALDR", Capacity = 1 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000003"), Name = "A TİMİ", Code = "A", Capacity = 25 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000004"), Name = "B TİMİ", Code = "B", Capacity = 15 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000005"), Name = "C TİMİ", Code = "C", Capacity = 20 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000006"), Name = "D TİMİ", Code = "D", Capacity = 18 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000007"), Name = "ARAMA", Code = "ARA", Capacity = 10 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000008"), Name = "MEDİKAL", Code = "MED", Capacity = 8 },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000009"), Name = "LOJİSTİK", Code = "LOG", Capacity = 12 }
    ];
}
