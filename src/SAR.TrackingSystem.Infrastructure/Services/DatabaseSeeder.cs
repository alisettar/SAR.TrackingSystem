using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Domain.SeedData;
using SAR.TrackingSystem.Infrastructure.Persistence;

namespace SAR.TrackingSystem.Infrastructure.Services;

public static class DatabaseSeeder
{
    // Backward compatibility overload
    public static async Task SeedAsync(SarDbContext context)
        => await SeedAsync(context, null);

    public static async Task SeedAsync(SarDbContext context, string? jsonFilePath = null)
    {
        ProcessedSeedData? processedData = null;
        
        // Tek seferde JSON'u işle (duplikasyonu önle)
        if (!string.IsNullOrEmpty(jsonFilePath) && File.Exists(jsonFilePath))
        {
            processedData = JsonSeedProcessor.ProcessJsonFile(jsonFilePath);
        }
        
        // Seed Teams - JSON'dan veya default'tan
        var teams = processedData?.Teams ?? DefaultSeedData.DefaultTeams;
        foreach (var team in teams)
        {
            var existingTeam = await context.Teams.FirstOrDefaultAsync(t => t.Code == team.Code);
            if (existingTeam == null)
            {
                context.Teams.Add(team);
            }
        }
        await context.SaveChangesAsync();
        
        // Seed Sectors (değişmez)
        foreach (var sector in DefaultSeedData.Sectors)
        {
            var existingSector = await context.Sectors.FirstOrDefaultAsync(s => s.Code == sector.Code);
            if (existingSector == null)
            {
                context.Sectors.Add(sector);
            }
        }
        await context.SaveChangesAsync();

        // Seed Volunteers - önceden işlenmiş data'yı kullan
        if (processedData?.Volunteers.Count != 0 == true)
        {
            // Team lookup için gerçek database ID'leri al
            var teamNameToId = await context.Teams
                .ToDictionaryAsync(t => t.Name, t => t.Id);
            
            foreach (var volunteer in processedData.Volunteers)
            {
                var existingVolunteer = await context.Volunteers
                    .FirstOrDefaultAsync(v => v.QRId == volunteer.QRId);
                if (existingVolunteer == null)
                {
                    // Team ID'yi gerçek database ID ile güncelle
                    var teamName = teams.First(t => t.Id == volunteer.TeamId).Name;
                    volunteer.TeamId = teamNameToId[teamName];
                    
                    context.Volunteers.Add(volunteer);
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
