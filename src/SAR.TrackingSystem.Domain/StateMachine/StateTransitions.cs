using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Domain.StateMachine;

/// <summary>
/// SAR State Machine for volunteer movement validation
/// Defines allowed state transitions in SAR operations
/// </summary>
public static class StateTransitions
{
    private static readonly Dictionary<VolunteerState, VolunteerState[]> _allowedTransitions = new()
    {
        [VolunteerState.NotEntered] = [VolunteerState.InHub],
        [VolunteerState.InHub] = [VolunteerState.InSector, VolunteerState.Exited],
        [VolunteerState.InSector] = [VolunteerState.InHub, VolunteerState.InSector],
        [VolunteerState.Exited] = [VolunteerState.InHub] // Re-entry allowed
    };

    /// <summary>
    /// Validates if state transition is allowed
    /// </summary>
    public static bool IsValidTransition(VolunteerState from, VolunteerState to)
        => _allowedTransitions[from].Contains(to);

    /// <summary>
    /// Gets volunteer state based on sector configuration
    /// </summary>
    public static VolunteerState GetStateFromSector(string? sectorCode, SectorConfiguration config)
    {
        if (string.IsNullOrEmpty(sectorCode)) return VolunteerState.NotEntered;
        if (sectorCode == config.HubCode) return VolunteerState.InHub;
        return VolunteerState.InSector;
    }

    /// <summary>
    /// Gets all allowed target states from current state
    /// </summary>
    public static VolunteerState[] GetAllowedTargetStates(VolunteerState currentState)
        => _allowedTransitions[currentState];

    /// <summary>
    /// Gets validation error message for invalid transitions
    /// </summary>
    public static string GetTransitionError(VolunteerState from, VolunteerState to)
    {
        return (from, to) switch
        {
            (VolunteerState.NotEntered, var target) when target != VolunteerState.InHub 
                => "İlk hareket BoO'ya yapılmalıdır.",
            (VolunteerState.InSector, VolunteerState.Exited) 
                => "Çıkış sadece BoO'dan yapılabilir.",
            (VolunteerState.Exited, var target) when target != VolunteerState.InHub 
                => "Yeniden giriş sadece BoO'ya yapılabilir.",
            (_, VolunteerState.NotEntered) 
                => "NotEntered durumuna geri dönüş yapılamaz.",
            _ => "Geçersiz durum geçişi."
        };
    }
}
