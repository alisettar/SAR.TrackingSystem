using Microsoft.EntityFrameworkCore;
using SAR.TrackingSystem.Infrastructure.Persistence;
using SAR.TrackingSystem.UnitTests.Factories;

namespace SAR.TrackingSystem.UnitTests.Integration;

public class DatabaseIntegrationTests : IDisposable
{
    private readonly SarDbContext _context;
    private const string _dbPath = @"C:\Users\Alisettar\source\repos\SAR.TrackingSystem\src\SAR.TrackingSystem.Api\SarTrackingDb.db";

    public DatabaseIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<SarDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        _context = new SarDbContext(options);
    }

    [Fact]
    public async Task CreateFullDataSet_ShouldSucceed()
    {
        // Arrange
        var teams = TeamMockFactory.GetSampleTeams();
        var sectors = SectorMockFactory.GetSampleSectors();

        // Act - Create Teams
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act - Create Sectors
        _context.Sectors.AddRange(sectors);
        await _context.SaveChangesAsync();

        // Act - Create Volunteers
        var volunteers = VolunteerMockFactory.GetSampleVolunteers(teams);
        _context.Volunteers.AddRange(volunteers);
        await _context.SaveChangesAsync();

        // Act - Create Movements
        var movements = MovementMockFactory.GetSampleMovements(volunteers[0].Id, sectors);
        _context.Movements.AddRange(movements);
        await _context.SaveChangesAsync();

        // Assert
        var teamCount = await _context.Teams.CountAsync();
        var sectorCount = await _context.Sectors.CountAsync();
        var volunteerCount = await _context.Volunteers.CountAsync();
        var movementCount = await _context.Movements.CountAsync();

        teamCount.Should().BeGreaterThanOrEqualTo(7);
        sectorCount.Should().BeGreaterThanOrEqualTo(7);
        volunteerCount.Should().BeGreaterThanOrEqualTo(5);
        movementCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task CreateVolunteersWithTeams_ShouldHaveCorrectRelations()
    {
        // Arrange
        var existingTeamsCount = await _context.Teams.CountAsync();
        if (existingTeamsCount == 0)
        {
            var newTeams = TeamMockFactory.GetSampleTeams();
            _context.Teams.AddRange(newTeams);
            await _context.SaveChangesAsync();
        }

        var existingTeams = await _context.Teams.ToListAsync();
        var medikalTeam = existingTeams.FirstOrDefault(t => t.Code == "MEDIKAL");
        
        if (medikalTeam == null)
        {
            medikalTeam = TeamMockFactory.GetMedikalTeam();
            _context.Teams.Add(medikalTeam);
            await _context.SaveChangesAsync();
        }

        var testVolunteer = VolunteerMockFactory.GetMedikalVolunteer(medikalTeam.Id);
        _context.Volunteers.Add(testVolunteer);
        await _context.SaveChangesAsync();

        // Act
        var volunteerWithTeam = await _context.Volunteers
            .Include(v => v.Team)
            .FirstAsync(v => v.Id == testVolunteer.Id);

        // Assert
        volunteerWithTeam.Team.Should().NotBeNull();
        volunteerWithTeam.Team.Code.Should().Be("MEDIKAL");
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
