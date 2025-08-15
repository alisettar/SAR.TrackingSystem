using SAR.TrackingSystem.Domain.Entities;
using System.Text.Json;

namespace SAR.TrackingSystem.Domain.SeedData;

public static class JsonSeedProcessor
{
    private static JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ProcessedSeedData ProcessJsonFile(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
            throw new FileNotFoundException($"JSON file not found: {jsonFilePath}");

        var jsonContent = File.ReadAllText(jsonFilePath);
        return ProcessJsonContent(jsonContent);
    }

    public static ProcessedSeedData ProcessJsonContent(string jsonContent)
    {
        var jsonData = JsonSerializer.Deserialize<List<VolunteerJsonData>>(jsonContent, jsonOptions);

        if (jsonData == null || jsonData.Count == 0)
            return new ProcessedSeedData();

        var result = new ProcessedSeedData();

        // Get existing default team codes to avoid collision
        var usedCodes = DefaultSeedData.DefaultTeams.Select(t => t.Code).ToHashSet();

        // Process Teams - unique teams from JSON
        var uniqueTeamData = jsonData
            .Where(x => !string.IsNullOrWhiteSpace(x.Ekip))
            .GroupBy(x => new { x.Ekip })
            .ToList();

        foreach (var group in uniqueTeamData)
        {
            var baseCode = GenerateTeamCode(group.Key.Ekip);
            var uniqueCode = EnsureUniqueCode(baseCode, usedCodes);
            usedCodes.Add(uniqueCode);

            // TODO: Aynı Ekipte birden fazla şehir bilgisi varsa en çok geçeni alıyoruz. Buna bakılabilir
            var mostCommonCity = group
               .Where(x => !string.IsNullOrWhiteSpace(x.Şehir))
               .GroupBy(x => x.Şehir)
               .OrderByDescending(g => g.Count())
               .FirstOrDefault()?.Key;

            var team = new Team(
                code: uniqueCode,
                name: group.Key.Ekip,
                city: mostCommonCity
            );

            result.Teams.Add(team);
        }

        // Process Volunteers 
        var teamLookup = result.Teams.ToDictionary(t => t.Name, t => t.Id);

        foreach (var item in jsonData.Where(x => !string.IsNullOrWhiteSpace(x.AdSoyad) &&
                                                 !string.IsNullOrWhiteSpace(x.QRId) &&
                                                 teamLookup.ContainsKey(x.Ekip) &&
                                                 !x.AdSoyad.Trim().Equals("İsim Kismi Boş", StringComparison.OrdinalIgnoreCase)))
        {
            var volunteer = Volunteer.Create(
                fullName: item.AdSoyad.Trim(),
                teamId: teamLookup[item.Ekip],
                qrId: item.QRId.Trim(),
                role: !string.IsNullOrWhiteSpace(item.Görev) ? item.Görev : "Ekip Üyesi" // Default role
            );

            result.Volunteers.Add(volunteer);
        }

        return result;
    }

    private static string EnsureUniqueCode(string baseCode, HashSet<string> usedCodes)
    {
        if (!usedCodes.Contains(baseCode))
            return baseCode;

        var counter = 1;
        string uniqueCode;
        do
        {
            uniqueCode = $"{baseCode}{counter}";
            counter++;
        }
        while (usedCodes.Contains(uniqueCode));

        return uniqueCode;
    }

    private static string GenerateTeamCode(string teamName)
    {
        // Simple code generation logic
        var words = teamName.Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 1)
            return words[0].Length >= 3 ? words[0][..3].ToUpper() : words[0].ToUpper();

        return string.Join("", words.Take(3).Select(w => w[0])).ToUpper();
    }
}
