using SAR.TrackingSystem.Domain.BaseClasses;
using SAR.TrackingSystem.Domain.Configuration;
using SAR.TrackingSystem.Domain.Enums;

namespace SAR.TrackingSystem.Domain.Entities;

public class Movement : Entity
{
    public Guid VolunteerId { get; set; }
    public Guid? FromSectorId { get; set; }
    public Guid ToSectorId { get; set; }
    public DateTime MovementTime { get; set; } = DateTime.Now;
    public MovementType Type { get; set; }
    public bool IsGroupMovement { get; set; }
    public Guid? GroupId { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Volunteer Volunteer { get; set; } = null!;
    public Sector? FromSector { get; set; }
    public Sector ToSector { get; set; } = null!;

    /// <summary>
    /// Creates a new Movement instance with the specified parameters.
    /// </summary>
    public static Movement Create(
        Guid volunteerId,
        Guid? fromSectorId,
        Guid toSectorId,
        MovementType type,
        bool isGroupMovement = false,
        Guid? groupId = null,
        string? notes = null)
    {
        return new Movement
        {
            VolunteerId = volunteerId,
            FromSectorId = fromSectorId,
            ToSectorId = toSectorId,
            MovementTime = DateTime.Now,
            Type = type,
            IsGroupMovement = isGroupMovement,
            GroupId = groupId,
            Notes = notes
        };
    }

    /// <summary>
    /// SAR Tracking System Business Rules for Movement Validation
    /// These rules ensure proper operational flow in Search and Rescue operations.
    /// </summary>
    public static class BusinessRules
    {
        /// <summary>
        /// Rule 1: İntikal (Entry) Validation
        /// Validates entry movements from ALAN_DIŞI to BOO.
        /// Entry is allowed only if:
        /// 1. Volunteer has no previous movements (first time entry), OR
        /// 2. Last movement was BOO → ÇIKIŞ (re-entry after exit)
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="hasExistingMovements">Whether volunteer has any previous movements</param>
        /// <param name="lastMovementFromCode">Last movement's source sector code</param>
        /// <param name="lastMovementToCode">Last movement's destination sector code</param>
        /// <param name="config">Sector configuration</param>
        /// <returns>True if entry movement is valid</returns>
        public static bool IsValidEntry(
            string? fromSectorCode,
            string toSectorCode,
            bool hasExistingMovements,
            string? lastMovementFromCode,
            string? lastMovementToCode,
            SectorConfiguration config)
        {
            // Only validate ALAN_DIŞI → BOO movements
            if (fromSectorCode == config.EntryCode && toSectorCode == config.HubCode)
            {
                if (!hasExistingMovements)
                {
                    // İlk giriş - OK
                    return true;
                }
                else
                {
                    // Yeniden giriş - sadece son hareket BOO → ÇIKIŞ ise
                    return lastMovementFromCode == config.HubCode && lastMovementToCode == config.ExitCode;
                }
            }
            
            // ALAN_DIŞI'ndan sadece ilk giriş veya çıkış sonrası giriş yapılabilir
            if (fromSectorCode == config.EntryCode)
            {
                if (hasExistingMovements && lastMovementToCode != config.ExitCode)
                {
                    return false; // Alan dışından sadece çıkış sonrası giriş
                }
            }

            return true; // Other movement types not restricted by this rule
        }

        /// <summary>
        /// Rule 2: Transfer Validation (Hub Model)
        /// Validates that all sector-to-sector movements must go through the Hub sector.
        /// Direct sector-to-sector transfers are prohibited for operational control.
        /// Example: E-1 → E-2 is invalid, must be E-1 → BOO → E-2
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="config">Sector configuration containing Hub code</param>
        /// <returns>True if transfer follows hub model</returns>
        public static bool IsValidTransfer(string fromSectorCode, string toSectorCode, SectorConfiguration config)
        {
            // Rule 2: Sektör geçişleri HUB üzerinden (Hub model)
            if (fromSectorCode != config.HubCode && toSectorCode != config.HubCode)
            {
                // Sektör → Sektör yasak (must go through Hub)
                return false;
            }
            return true;
        }

        /// <summary>
        /// Rule 3: Exit Validation
        /// Validates that volunteers can only exit from the Hub sector.
        /// Direct exits from operational sectors are prohibited for proper checkout procedures.
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="config">Sector configuration containing Hub and Exit codes</param>
        /// <returns>True if exit is from valid sector</returns>
        public static bool IsValidExit(string fromSectorCode, string toSectorCode, SectorConfiguration config)
        {
            // Rule 3: Sadece HUB → EXIT
            if (toSectorCode == config.ExitCode)
            {
                return fromSectorCode == config.HubCode;
            }
            return true;
        }

        /// <summary>
        /// Rule 4: Group Movement Validation
        /// Validates that group movements have a proper GroupId for coordination tracking.
        /// </summary>
        /// <param name="isGroupMovement">Whether this is a group movement</param>
        /// <param name="groupId">Group identifier for coordinated movements</param>
        /// <returns>True if group movement has valid GroupId</returns>
        public static bool IsValidGroupMovement(bool isGroupMovement, Guid? groupId)
        {
            // Rule 4: Grup hareket ise GroupId zorunlu
            if (isGroupMovement)
            {
                return groupId.HasValue && groupId != Guid.Empty;
            }
            return true;
        }

        /// <summary>
        /// Rule 5: Exit Requires Entry Validation
        /// Validates that volunteers cannot exit without having entered first.
        /// Prevents checkout without proper check-in procedures.
        /// </summary>
        public static bool IsValidExitRequiresEntry(
            string toSectorCode,
            bool hasExistingMovements,
            bool hasEntryMovement,
            SectorConfiguration config)
        {
            // Rule 5: Giriş hareketi olmadan çıkış yapılamaz
            if (toSectorCode == config.ExitCode)
            {
                return hasExistingMovements && hasEntryMovement;
            }
            return true;
        }

        /// <summary>
        /// Rule 6: Sector Transfer Validation (BOO → Sector)
        /// Validates that volunteers can only go to sectors from BOO hub.
        /// </summary>
        public static bool IsValidSectorTransfer(
            string fromSectorCode,
            string toSectorCode,
            string? lastMovementToCode,
            SectorConfiguration config)
        {
            // Sadece BOO'dan sektöre geçiş kontrolü
            if (fromSectorCode == config.HubCode && 
                toSectorCode != config.HubCode && 
                toSectorCode != config.ExitCode && 
                toSectorCode != config.EntryCode)
            {
                // Gönüllü şu anda BOO'da olmalı
                return lastMovementToCode == config.HubCode;
            }
            return true;
        }

        /// <summary>
        /// Rule 7: Return to Hub Validation (Sector → BOO)
        /// Validates that volunteers can return to BOO from any sector.
        /// </summary>
        public static bool IsValidReturnToHub(
            string fromSectorCode,
            string toSectorCode,
            string? lastMovementToCode,
            SectorConfiguration config)
        {
            // Sektörden BOO'ya dönüş kontrolü
            if (toSectorCode == config.HubCode && 
                fromSectorCode != config.HubCode &&
                fromSectorCode != config.ExitCode &&
                fromSectorCode != config.EntryCode)
            {
                // Gönüllü şu anda bir sektörde olmalı (BOO'da değil)
                return lastMovementToCode != config.HubCode;
            }
            return true;
        }

        /// <summary>
        /// Rule 8: Same Sector Movement Validation
        /// Validates that volunteers cannot move to the same sector they are currently in.
        /// This prevents unnecessary movement records and maintains data integrity.
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <returns>True if movement is to a different sector</returns>
        public static bool IsValidSameSectorMovement(string? fromSectorCode, string toSectorCode)
        {
            // Rule 8: Aynı sektörden aynı sektöre hareket yapılamaz
            if (!string.IsNullOrEmpty(fromSectorCode))
            {
                return fromSectorCode != toSectorCode;
            }
            return true; // İlk hareket için geçerli değil
        }

        /// <summary>
        /// Comprehensive validation that checks all business rules and returns detailed error message.
        /// Used by validators to provide specific feedback about rule violations.
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="hasExistingMovements">Whether volunteer has previous movements</param>
        /// <param name="lastMovementFromCode">Last movement's source sector code</param>
        /// <param name="lastMovementToCode">Last movement's destination sector code</param>
        /// <param name="isGroupMovement">Whether this is a group movement</param>
        /// <param name="groupId">Group identifier</param>
        /// <param name="hasEntryMovement">Whether volunteer has at least one entry movement</param>
        /// <param name="config">Sector configuration</param>
        /// <returns>Empty string if valid, error message if invalid</returns>
        public static string GetValidationError(
            string? fromSectorCode,
            string toSectorCode,
            bool hasExistingMovements,
            string? lastMovementFromCode,
            string? lastMovementToCode,
            bool isGroupMovement,
            Guid? groupId,
            bool hasEntryMovement,
            SectorConfiguration config)
        {
            // Rule 8 Check: Same sector movement validation (first check for better UX)
            if (!IsValidSameSectorMovement(fromSectorCode, toSectorCode))
                return "Aynı sektörde kalınamaz. Farklı bir sektör seçiniz.";

            if (!IsValidEntry(fromSectorCode, toSectorCode, hasExistingMovements, lastMovementFromCode, lastMovementToCode, config))
            {
                if (!hasExistingMovements)
                    return $"İlk hareket {config.EntryCode}'ndan {config.HubCode}'ya yapılmalıdır.";
                else
                    return $"ALAN_DIŞI'ndan hareket için önce sistemden çıkış yapınız.";
            }

            // COMMENTED OUT: Hub transfer rule disabled per request
            // if (!IsValidTransfer(fromSectorCode ?? "", toSectorCode, config))
            //     return $"Sektör geçişleri {config.HubCode} üzerinden yapılmalıdır.";

            if (!IsValidExit(fromSectorCode ?? "", toSectorCode, config))
                return $"Çıkış sadece {config.HubCode}'dan yapılabilir.";

            if (!IsValidExitRequiresEntry(toSectorCode, hasExistingMovements, hasEntryMovement, config))
                return "Giriş hareketi olmadan çıkış yapılamaz.";

            if (!IsValidSectorTransfer(fromSectorCode ?? "", toSectorCode, lastMovementToCode, config))
                return $"Sektöre gitmek için önce {config.HubCode}'da olmalısınız.";

            if (!IsValidReturnToHub(fromSectorCode ?? "", toSectorCode, lastMovementToCode, config))
                return $"{config.HubCode}'ya dönmek için bir sektörde olmalısınız.";

            if (!IsValidGroupMovement(isGroupMovement, groupId))
                return "Grup hareketi için GroupId zorunludur.";

            return string.Empty;
        }
    }
}