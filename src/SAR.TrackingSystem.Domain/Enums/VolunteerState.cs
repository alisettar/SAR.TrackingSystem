namespace SAR.TrackingSystem.Domain.Enums;

/// <summary>
/// Volunteer states in SAR operations workflow
/// </summary>
public enum VolunteerState
{
    /// <summary>
    /// Volunteer has not entered the operation area yet
    /// </summary>
    NotEntered = 0,
    
    /// <summary>
    /// Volunteer is in Hub (BoO - Base of Operations)
    /// </summary>
    InHub = 1,
    
    /// <summary>
    /// Volunteer is in a sector (E-1, E-2, E2-A, E2-B, etc.)
    /// </summary>
    InSector = 2,
    
    /// <summary>
    /// Volunteer has exited the operation
    /// </summary>
    Exited = 3
}
