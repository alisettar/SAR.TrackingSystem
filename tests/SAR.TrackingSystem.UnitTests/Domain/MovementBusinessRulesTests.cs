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
            EntryCode = "ALAN_DIŞI",
            HubCode = "BOO", 
            ExitCode = "ÇIKIŞ"
        };
    }

    [Fact]
    public void IsValidEntry_FirstMovement_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidEntry("ALAN_DIŞI", "BOO", false, _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidEntry_NotFirstMovement_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidEntry("BOO", "E-1", true, _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidTransfer_HubToSector_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidTransfer("BOO", "E-1", _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidTransfer_SectorToHub_ShouldReturnTrue()
    {
        // Act
        var result = Movement.BusinessRules.IsValidTransfer("E-1", "BOO", _config);

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
        var result = Movement.BusinessRules.IsValidExit("BOO", "ÇIKIŞ", _config);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidExit_FromSector_ShouldReturnFalse()
    {
        // Act
        var result = Movement.BusinessRules.IsValidExit("E-1", "ÇIKIŞ", _config);

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
