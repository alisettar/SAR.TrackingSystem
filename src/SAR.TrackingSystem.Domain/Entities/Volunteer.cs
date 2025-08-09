using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Volunteer : Entity
{
    public string FullName { get; set; } = string.Empty;
    public string QRId { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string Role { get; set; } = string.Empty;
    
    // Navigation
    public Team Team { get; set; } = null!;
    public List<Movement> Movements { get; set; } = new();

    public static Volunteer Create(
        string fullName,
        Guid teamId,
        string qrId,
        string role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ad Soyad zorunludur", nameof(fullName));
        if (string.IsNullOrWhiteSpace(qrId))
            throw new ArgumentException("QR ID zorunludur", nameof(qrId));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Görev zorunludur", nameof(role));
        if (teamId == Guid.Empty)
            throw new ArgumentException("Ekip seçimi zorunludur", nameof(teamId));
            
        return new Volunteer
        {
            FullName = fullName.Trim(),
            TeamId = teamId,
            QRId = qrId.Trim(),
            Role = role.Trim()
        };
    }

    public static Volunteer Update(
        Volunteer currentVolunteer,
        string fullName,
        Guid teamId,
        string qrId,
        string role)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Ad Soyad zorunludur", nameof(fullName));
        if (string.IsNullOrWhiteSpace(qrId))
            throw new ArgumentException("QR ID zorunludur", nameof(qrId));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Görev zorunludur", nameof(role));
        if (teamId == Guid.Empty)
            throw new ArgumentException("Ekip seçimi zorunludur", nameof(teamId));
            
        currentVolunteer.FullName = fullName.Trim();
        currentVolunteer.TeamId = teamId;
        currentVolunteer.QRId = qrId.Trim();
        currentVolunteer.Role = role.Trim();

        return currentVolunteer;
    }
}
