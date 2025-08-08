namespace SAR.TrackingSystem.Domain.Enums;

public enum MovementType
{
    Entry = 1,      // İlk Giriş (ALAN_DIŞI → BOO)
    Transfer = 2,   // Transfer (BOO → Sektör veya Sektör → BOO)
    Exit = 3        // Çıkış (Sektör → ÇIKIŞ)
}
