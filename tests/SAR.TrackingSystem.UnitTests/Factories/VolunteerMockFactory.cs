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

    private static readonly string[] BloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "0+", "0-" };
    
    private static readonly Random Random = new();

    public static List<Volunteer> GetSampleVolunteers(List<Team> teams)
    {
        var volunteers = new List<Volunteer>();
        
        for (int i = 0; i < 2000; i++)
        {
            var team = teams[Random.Next(teams.Count)];
            var firstName = FirstNames[Random.Next(FirstNames.Length)];
            var lastName = LastNames[Random.Next(LastNames.Length)];
            var fullName = $"{firstName} {lastName}";
            
            var tcKimlik = GenerateTcKimlik(i);
            var bloodType = BloodTypes[Random.Next(BloodTypes.Length)];
            var phone = GeneratePhone();
            var emergencyName = $"Acil {firstName}";
            var emergencyPhone = GeneratePhone();
            var buddy1 = FirstNames[Random.Next(FirstNames.Length)];
            var buddy2 = FirstNames[Random.Next(FirstNames.Length)];
            
            volunteers.Add(Volunteer.Create(tcKimlik, fullName, team.Id, bloodType, 
                phone, emergencyName, emergencyPhone, buddy1, buddy2));
        }
        
        return volunteers;
    }
    
    private static long GenerateTcKimlik(int index)
    {
        // Generate realistic TC Kimlik numbers starting from 10000000000
        return 10000000000L + index;
    }
    
    private static string GeneratePhone()
    {
        return $"0555{Random.Next(1000000, 9999999)}";
    }

    public static Volunteer GetTestVolunteer(Guid teamId)
    {
        return Volunteer.Create(11111111111, "Test Ekip Üyesi", teamId, "A+", "05551111111", "Emergency Test", "05559999999", "Buddy1", "Buddy2");
    }

    public static Volunteer GetMedikalVolunteer(Guid medikalTeamId)
    {
        return Volunteer.Create(99999999999, "Dr. Test Doktor", medikalTeamId, "0-", "05559999999", "Emergency Dr", "05559999998", "Hemşire1", "Hemşire2");
    }
}
