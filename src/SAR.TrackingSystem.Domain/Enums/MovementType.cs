namespace SAR.TrackingSystem.Domain.Enums;

public enum MovementType
{
    Entry = 1,      // İlk Giriş (ALAN_DIŞI → BoO)
    Transfer = 2,   // Transfer (BoO → Sektör veya Sektör → BoO)
    Exit = 3        // Çıkış (Sektör → ÇIKIŞ)
}
