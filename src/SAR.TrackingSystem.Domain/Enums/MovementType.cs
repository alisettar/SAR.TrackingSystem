namespace SAR.TrackingSystem.Domain.Enums;

public enum MovementType
{
    Entry = 1,      // İlk Giriş (null → BOO)
    Transfer = 2,   // Transfer (Sektör → Sektör)
    Exit = 3        // Çıkış (Sektör → ÇIKIŞ)
}
