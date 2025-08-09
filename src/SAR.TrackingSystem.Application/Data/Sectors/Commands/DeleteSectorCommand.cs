using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using FluentValidation;
using Microsoft.Extensions.Options;
using SAR.TrackingSystem.Domain.Configuration;

namespace SAR.TrackingSystem.Application.Data.Sectors.Commands;

public sealed record DeleteSectorCommand(Guid SectorId) : IRequest<bool>;

public sealed class DeleteSectorCommandHandler(
    ISectorRepository sectorRepository,
    IOptions<SectorConfiguration> config) : IRequestHandler<DeleteSectorCommand, bool>
{
    private readonly SectorConfiguration _config = config.Value;

    public async Task<bool> Handle(DeleteSectorCommand request, CancellationToken cancellationToken)
    {
        // Get sector first
        var sector = await sectorRepository.GetByIdAsync(request.SectorId, cancellationToken);
        
        if (sector == null)
        {
            throw new ValidationException("Sektör bulunamadı.");
        }

        // Critical sector protection - use IsCriticalForBusinessRules from domain
        if (sector.IsCriticalForBusinessRules)
        {
            throw new ValidationException("Bu sektör sistem için kritik olduğu için silinemez.");
        }

        // Alternative check using configuration (backup)
        if (_config.CriticalSectorCodes.Contains(sector.Code.ToUpper()))
        {
            throw new ValidationException("Bu sektör sistem için kritik olduğu için silinemez.");
        }

        // Delete sector
        await sectorRepository.DeleteAsync(sector, cancellationToken);
        return true;
    }
}

public sealed class DeleteSectorCommandValidator : AbstractValidator<DeleteSectorCommand>
{
    public DeleteSectorCommandValidator()
    {
        RuleFor(x => x.SectorId)
            .NotEmpty()
            .WithMessage("Sektör ID'si gereklidir.");
    }
}
