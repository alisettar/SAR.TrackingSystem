using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Options;

using SAR.TrackingSystem.Domain.Configuration;

namespace SAR.TrackingSystem.Application.Data.Sectors.Commands;

public sealed record CreateSectorCommand(SectorRequest Request) : IRequest<Guid>;

public sealed class CreateSectorCommandHandler(
    ISectorRepository sectorRepository,
    IOptions<SectorConfiguration> config) : IRequestHandler<CreateSectorCommand, Guid>
{
    private readonly SectorConfiguration _config = config.Value;

    public async Task<Guid> Handle(CreateSectorCommand request, CancellationToken cancellationToken)
    {
        // Business Rules Validation
        var existingSector = await sectorRepository.GetByCodeAsync(request.Request.Code, cancellationToken);

        if (existingSector != null)
        {
            throw new ValidationException("Bu kodla kayıtlı başka bir sektör zaten var.");
        }

        // Critical sector codes check
        if (_config.CriticalSectorCodes.Contains(request.Request.Code.ToUpper()))
        {
            throw new ValidationException("Bu sektör kodu sistem için kritiktir ve özel izin gerektirir.");
        }

        // Create Sector
        var sector = new Sector(
            code: request.Request.Code,
            name: request.Request.Name,
            isActive: request.Request.IsActive,
            isEntryPoint: request.Request.IsEntryPoint,
            isExitPoint: request.Request.IsExitPoint);

        await sectorRepository.AddAsync(sector, cancellationToken);
        return sector.Id;
    }
}

public sealed class CreateSectorCommandValidator : AbstractValidator<CreateSectorCommand>
{
    public CreateSectorCommandValidator()
    {
        RuleFor(x => x.Request.Code)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Sektör kodu maksimum 10 karakter olmalıdır.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Sektör adı maksimum 100 karakter olmalıdır.");

        RuleFor(x => x.Request)
            .Must(x => !(x.IsEntryPoint && x.IsExitPoint))
            .WithMessage("Bir sektör hem giriş hem de çıkış noktası olamaz.");
    }
}