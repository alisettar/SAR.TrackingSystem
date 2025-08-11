namespace SAR.TrackingSystem.Domain.Enums;

/// <summary>
/// Movement types for SAR operations
/// </summary>
public enum MovementType
{
    /// <summary>
    /// Entry movement (null -> BoO)
    /// </summary>
    Entry = 1,
    
    /// <summary>
    /// Transfer between sectors or to/from hub
    /// </summary>
    Transfer = 2,
    
    /// <summary>
    /// Exit movement (BoO -> null)
    /// </summary>
    Exit = 3,
    
    /// <summary>
    /// Re-entry after exit (null -> BoO)
    /// </summary>
    ReEntry = 4
}
