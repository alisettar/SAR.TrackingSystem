namespace SAR.TrackingSystem.Web.Models;

public class MovementHistoryViewModel
{
    public Guid Id { get; set; }
    public DateTime MovementTime { get; set; }
    public string FromSector { get; set; } = string.Empty;
    public string ToSector { get; set; } = string.Empty;
    public string MovementType { get; set; } = string.Empty;
    public bool IsGroupMovement { get; set; }
    public string Notes { get; set; } = string.Empty;
    
    public string TimeFormatted => MovementTime.ToString("HH:mm");
    public string DateFormatted => MovementTime.ToString("dd.MM.yyyy");
    public string MovementDescription => GetMovementDescription();
    
    public string TimelineIcon => MovementType switch
    {
        "Giriş" => "bi-arrow-right-circle-fill text-success",
        "Çıkış" => "bi-arrow-left-circle-fill text-danger",
        "Transfer" => "bi-arrow-left-right text-primary",
        _ => "bi-arrow-left-right text-secondary"
    };
    
    private string GetMovementDescription()
    {
        return MovementType switch
        {
            "Giriş" => $"Alan Dışı → {ToSector}",
            "Çıkış" => $"{FromSector} → Alan Dışı",
            "Transfer" => $"{FromSector} → {ToSector}",
            _ => $"{FromSector} → {ToSector}"
        };
    }
}
