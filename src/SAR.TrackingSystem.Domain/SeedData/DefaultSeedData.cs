using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Domain.SeedData;

public static class DefaultSeedData
{
    public static List<Sector> Sectors =>
    [
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000002"), Code = "BoO", Name = "Base of Operations", IsEntryPoint = true, IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000003"), Code = "E-1", Name = "Sektör E-1", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000004"), Code = "E-2", Name = "Sektör E-2", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000005"), Code = "E2-A", Name = "Sektör E2-A", IsActive = true },
        new Sector { Id = new Guid("00000000-0000-0000-0000-000000000006"), Code = "E2-B", Name = "Sektör E2-B", IsActive = true }
    ];

    public static List<Team> Teams =>
    [
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000001"), Name = "EKİP LİDERİ", Code = "LDR" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000002"), Name = "LİDER YARDIMCISI", Code = "ALDR" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000003"), Name = "A TİMİ", Code = "A" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000004"), Name = "B TİMİ", Code = "B" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000005"), Name = "C TİMİ", Code = "C" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000006"), Name = "D TİMİ", Code = "D" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000007"), Name = "ARAMA", Code = "ARA" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000008"), Name = "MEDİKAL", Code = "MED" },
        new Team { Id = new Guid("11111111-0000-0000-0000-000000000009"), Name = "LOJİSTİK", Code = "LOG" }
    ];
}
