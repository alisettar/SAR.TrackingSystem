using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Factories;

public static class VolunteerMockFactory
{
    public static List<Volunteer> GetSampleVolunteers(List<Team> teams)
    {
        var teamA = teams.First(t => t.Code == "A");
        var teamB = teams.First(t => t.Code == "B");
        var medikal = teams.First(t => t.Code == "MEDIKAL");

        return
        [
            Volunteer.Create(12345678901, "Ahmet Yılmaz", teamA.Id, "A+", "05551234567", "Emergency1", "05559999991", "Ali", "Veli"),
            Volunteer.Create(12345678902, "Mehmet Öz", teamA.Id, "0+", "05551234568", "Emergency2", "05559999992", "Can", "Cem"),
            Volunteer.Create(12345678903, "Fatma Kaya", teamB.Id, "AB+", "05551234569", "Emergency3", "05559999993", "Ayşe", "Zeynep"),
            Volunteer.Create(12345678904, "Dr. Kemal Arslan", medikal.Id, "B-", "05551234570", "Emergency4", "05559999994", "Selim", "Taner"),
            Volunteer.Create(12345678905, "Elif Şahin", teamB.Id, "A-", "05551234571", "Emergency5", "05559999995", "Seda", "Gül")
        ];
    }

    public static Volunteer GetTestVolunteer(Guid teamId)
    {
        return Volunteer.Create(11111111111, "Test Gönüllü", teamId, "A+", "05551111111", "Emergency Test", "05559999999", "Buddy1", "Buddy2");
    }

    public static Volunteer GetMedikalVolunteer(Guid medikalTeamId)
    {
        return Volunteer.Create(99999999999, "Dr. Test Doktor", medikalTeamId, "0-", "05559999999", "Emergency Dr", "05559999998", "Hemşire1", "Hemşire2");
    }
}
