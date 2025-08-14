using System.ComponentModel.DataAnnotations;

namespace SAR.TrackingSystem.Application.Data.Sectors.Commands;

public record UpdateSectorCountsRequest(
    [Range(0, int.MaxValue, ErrorMessage = "Sağ çıkarılan sayısı 0 veya daha büyük olmalıdır")]
    int RescuedCount,
    
    [Range(0, int.MaxValue, ErrorMessage = "Ex çıkarılan sayısı 0 veya daha büyük olmalıdır")]
    int ExtricatedCount
);
