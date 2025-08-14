using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.Domain.SeedData;

public class VolunteerJsonData
{
    public string AdSoyad { get; set; } = string.Empty;
    public string QRId { get; set; } = string.Empty;
    public string Ekip { get; set; } = string.Empty;
    public string Şehir { get; set; } = string.Empty;
    public string Görev { get; set; } = string.Empty;
}

public class ProcessedSeedData
{
    public List<Team> Teams { get; set; } = new();
    public List<Volunteer> Volunteers { get; set; } = new();
}
