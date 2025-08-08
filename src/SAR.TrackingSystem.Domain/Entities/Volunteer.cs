using SAR.TrackingSystem.Domain.BaseClasses;

namespace SAR.TrackingSystem.Domain.Entities;

public class Volunteer : Entity
{
    public long TcKimlik { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid TeamId { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
    public string? Buddy1 { get; set; }
    public string? Buddy2 { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation
    public Team Team { get; set; } = null!;
    public List<Movement> Movements { get; set; } = new();

    public static Volunteer Create(
        long tcKimlik,
        string fullName,
        Guid teamId,
        string bloodType,
        string phone,
        string emergencyContactName,
        string emergencyContactPhone,
        string? buddy1 = null,
        string? buddy2 = null,
        bool isActive = true)
    {
        return new Volunteer
        {
            TcKimlik = tcKimlik,
            FullName = fullName,
            TeamId = teamId,
            BloodType = bloodType,
            Phone = phone,
            EmergencyContactName = emergencyContactName,
            EmergencyContactPhone = emergencyContactPhone,
            Buddy1 = buddy1,
            Buddy2 = buddy2,
            IsActive = isActive,
            CreatedAt = DateTime.Now
        };
    }

    public static Volunteer Update(
        Volunteer currentVolunteer,
        long tcKimlik,
        string fullName,
        Guid teamId,
        string bloodType,
        string phone,
        string emergencyContactName,
        string emergencyContactPhone,
        string? buddy1 = null,
        string? buddy2 = null,
        bool isActive = true)
    {
        currentVolunteer.TcKimlik = tcKimlik;
        currentVolunteer.FullName = fullName;
        currentVolunteer.TeamId = teamId;
        currentVolunteer.BloodType = bloodType;
        currentVolunteer.Phone = phone;
        currentVolunteer.EmergencyContactName = emergencyContactName;
        currentVolunteer.EmergencyContactPhone = emergencyContactPhone;
        currentVolunteer.Buddy1 = buddy1;
        currentVolunteer.Buddy2 = buddy2;
        currentVolunteer.IsActive = isActive;

        return currentVolunteer;
    }
}
