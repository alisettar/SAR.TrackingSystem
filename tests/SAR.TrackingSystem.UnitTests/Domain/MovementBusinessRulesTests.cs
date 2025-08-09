using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Domain.Entities;

namespace SAR.TrackingSystem.UnitTests.Domain;

public class MovementBusinessRulesTests
{
    private readonly SectorConfiguration _config;

    public MovementBusinessRulesTests()
    {
        _config = new SectorConfiguration
        {
            EntryCode = "Entry",
            HubCode = "BoO", 
            ExitCode = "Exit"
        };
    }

    [Fact]
    public void IsValidEntry_FirstMovement_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidEntry("Entry", "BoO", false, null, null, _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidEntry_NotFirstMovement_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidEntry("BoO", "E-1", true, null, null, _config);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsValidEntry_ReentryAfterExit_ShouldReturnTrue()
    {
        // Act - Last movement was BoO → Exit
        var result = Movement.BusinessRules.IsValidEntry("Entry", "BoO", true, "BoO", "Exit", _config);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsValidEntry_ReentryWithoutExit_ShouldReturnFalse()
    {
        // Act - Last movement was E-1 → BoO (not an exit)
        var result = Movement.BusinessRules.IsValidEntry("Entry", "BoO", true, "E-1", "BoO", _config);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidTransfer_HubToSector_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidTransfer("BoO", "E-1", _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidTransfer_SectorToHub_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidTransfer("E-1", "BoO", _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidTransfer_SectorToSector_ShouldReturnFalse()
    {
        // Act
        var result = Movement.BusinessRules.IsValidTransfer("E-1", "E-2", _config);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidExit_FromHub_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidExit("BoO", "Exit", _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidExit_FromSector_ShouldReturnFalse()
    {
        // Act
        var result = Movement.BusinessRules.IsValidExit("E-1", "Exit", _config);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidGroupMovement_WithGroupId_ShouldReturnTrue()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        // Act
        var result = Movement.BusinessRules.IsValidGroupMovement(true, groupId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidGroupMovement_WithoutGroupId_ShouldReturnFalse()
    {
        // Act
        var result = Movement.BusinessRules.IsValidGroupMovement(true, null);

        // Assert
        result.Should().BeFalse();
    }
}
