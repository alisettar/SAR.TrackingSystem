namespace SAR.TrackingSystem.Domain.Configuration;

public class SectorConfiguration
{
    public const string SectionName = "SectorSettings";
    public List<string> CriticalSectorCodes { get; set; } = new List<string>();

    public string EntryCode { get; set; } = "Entry";
    public string HubCode { get; set; } = "BoO";
    public string ExitCode { get; set; } = "Exit";
}