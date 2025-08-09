using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class VolunteerMockFactory
{
    private static readonly string[] FirstNames =
    [
        "Ahmet", "Mehmet", "Mustafa", "Ali", "Hasan", "Hüseyin", "İbrahim", "İsmail", "Ömer", "Osman",
        "Fatma", "Ayşe", "Emine", "Hatice", "Zeynep", "Elif", "Meryem", "Khadija", "Zümra", "Sümeyye",
        "Burak", "Emre", "Murat", "Serkan", "Tolga", "Kemal", "Selim", "Taner", "Yasin", "Yusuf",
        "Seda", "Gül", "Cansu", "Burcu", "Pınar", "Şebnem", "Nilgün", "Sevgi", "Dilek", "Fulya"
    ];

    private static readonly string[] LastNames = 
    [
        "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Yıldırım", "Öztürk", "Aydın", "Özdemir",
        "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Kara", "Koç", "Kurt", "Özkan", "Şimşek",
        "Erdoğan", "Ünal", "Keskin", "Başar", "Taş", "Polat", "Gül", "Karaca", "Güner", "Özer"
    ];

    private static readonly string[] Roles = 
    [
        "Lider", "Lider Yardımcısı", "Medik", "Teknisyen", "İletişim", "Lojistik", 
        "Kurtarma", "Arama", "K9", "Koordinatör", "Gözlemci"
    ];
    
    private static readonly Random Random = new();

    public static List<Volunteer> GetSampleVolunteers(List<Team> teams)
    {
        var volunteers = new List<Volunteer>();
        
        for (int i = 0; i < 200; i++)
        {
            var team = teams[Random.Next(teams.Count)];
            var firstName = FirstNames[Random.Next(FirstNames.Length)];
            var lastName = LastNames[Random.Next(LastNames.Length)];
            var fullName = $"{firstName} {lastName}";
            
            var qrId = GenerateQRId(i);
            var role = Roles[Random.Next(Roles.Length)]; // Her volunteer'ın role'ü olmalı
            
            volunteers.Add(Volunteer.Create(
                fullName: fullName,
                teamId: team.Id,
                qrId: qrId,
                role: role));
        }
        
        return volunteers;
    }
    
    private static string GenerateQRId(int index)
    {
        // Generate QR ID like QR001, QR002, etc.
        return $"QR{(index + 1):D3}";
    }

    public static Volunteer GetTestVolunteer(Guid teamId)
    {
        return Volunteer.Create(
            fullName: "Test Ekip Üyesi",
            teamId: teamId,
            qrId: "QR999",
            role: "Test Görevlisi");
    }

    public static Volunteer GetMedikalVolunteer(Guid medikalTeamId)
    {
        return Volunteer.Create(
            fullName: "Dr. Test Doktor",
            teamId: medikalTeamId,
            qrId: "QR888",
            role: "Doktor");
    }
}
