using FluentValidation;
using MediatR;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Sectors.Commands;

public record UpdateSectorCountsCommand(
    Guid SectorId,
    int RescuedCount,
    int ExtricatedCount
) : IRequest<bool>;

public class UpdateSectorCountsCommandHandler(
    ISectorRepository sectorRepository) : IRequestHandler<UpdateSectorCountsCommand, bool>
{
    public async Task<bool> Handle(UpdateSectorCountsCommand request, CancellationToken cancellationToken)
    {
        var sector = await sectorRepository.GetByIdAsync(request.SectorId, cancellationToken);
        if (sector == null)
        {
            return false;
        }

        // Additional business validation
        var total = request.RescuedCount + request.ExtricatedCount;
        if (sector.ExpectedVictimCount > 0 && total > sector.ExpectedVictimCount)
        {
            throw new ValidationException($"Toplam çıkarılan sayısı ({total}) beklenen afetzede sayısını ({sector.ExpectedVictimCount}) aşamaz.");
        }

        // Update counts
        sector.RescuedCount = Math.Max(0, request.RescuedCount);
        sector.ExtricatedCount = Math.Max(0, request.ExtricatedCount);
        sector.LastUpdated = DateTime.UtcNow;

        await sectorRepository.UpdateAsync(sector, cancellationToken);

        return true;
    }
}

//public class UpdateSectorCountsCommandValidator : AbstractValidator<UpdateSectorCountsCommand>
//{
//    private readonly ISectorRepository _sectorRepository;

//    public UpdateSectorCountsCommandValidator(ISectorRepository sectorRepository)
//    {
//        _sectorRepository = sectorRepository;

//        RuleFor(x => x.RescuedCount)
//            .GreaterThanOrEqualTo(0)
//            .WithMessage("Sağ çıkarılan sayısı 0 veya daha büyük olmalıdır.");

//        RuleFor(x => x.ExtricatedCount)
//            .GreaterThanOrEqualTo(0)
//            .WithMessage("Ex çıkarılan sayısı 0 veya daha büyük olmalıdır.");

//        RuleFor(x => x)
//            .MustAsync(async (command, cancellation) =>
//            {
//                var sector = await _sectorRepository.GetByIdAsync(command.SectorId, cancellation);
//                if (sector == null) return false;

//                var total = command.RescuedCount + command.ExtricatedCount;
//                return total <= sector.ExpectedVictimCount;
//            })
//            .WithMessage("Toplam çıkarılan sayısı beklenen afetzede sayısını aşamaz.")
//            .When(x => x.RescuedCount >= 0 && x.ExtricatedCount >= 0);
//    }
//}
