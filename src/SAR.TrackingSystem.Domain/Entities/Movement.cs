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
        /// Validates that the first movement for any volunteer must be from Entry sector to Hub sector.
        /// This ensures controlled entry and proper registration of all volunteers.
        /// </summary>
        /// <param name="fromSectorCode">Source sector code (null for first movement)</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="hasExistingMovements">Whether volunteer has any previous movements</param>
        /// <param name="config">Sector configuration containing Entry and Hub codes</param>
        /// <returns>True if entry movement is valid</returns>
        public static bool IsValidEntry(string? fromSectorCode, string toSectorCode, bool hasExistingMovements, SectorConfiguration config)
        {
            // Rule 1: İlk hareket mutlaka ENTRY → HUB olmalı
            if (!hasExistingMovements)
            {
                return fromSectorCode == config.EntryCode && toSectorCode == config.HubCode;
            }
            return true;
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
        /// Comprehensive validation that checks all business rules and returns detailed error message.
        /// Used by validators to provide specific feedback about rule violations.
        /// </summary>
        /// <param name="fromSectorCode">Source sector code</param>
        /// <param name="toSectorCode">Destination sector code</param>
        /// <param name="hasExistingMovements">Whether volunteer has previous movements</param>
        /// <param name="isGroupMovement">Whether this is a group movement</param>
        /// <param name="groupId">Group identifier</param>
        /// <param name="config">Sector configuration</param>
        /// <returns>Empty string if valid, error message if invalid</returns>
        public static string GetValidationError(
            string? fromSectorCode, 
            string toSectorCode, 
            bool hasExistingMovements,
            bool isGroupMovement,
            Guid? groupId,
            SectorConfiguration config)
        {
            if (!IsValidEntry(fromSectorCode, toSectorCode, hasExistingMovements, config))
                return $"İlk hareket {config.EntryCode}'ndan {config.HubCode}'ya yapılmalıdır.";

            if (!IsValidTransfer(fromSectorCode ?? "", toSectorCode, config))
                return $"Sektör geçişleri {config.HubCode} üzerinden yapılmalıdır.";

            if (!IsValidExit(fromSectorCode ?? "", toSectorCode, config))
                return $"Çıkış sadece {config.HubCode}'dan yapılabilir.";

            if (!IsValidGroupMovement(isGroupMovement, groupId))
                return "Grup hareketi için GroupId zorunludur.";

            return string.Empty;
        }
    }
}