using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using SAR.TrackingSystem.Domain.Configuration;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record CreateMovementCommand(MovementRequest Request) : IRequest<Guid>;

public sealed class CreateMovementCommandHandler(
    IMovementRepository movementRepository,
    ISectorRepository sectorRepository,
    IOptions<SectorConfiguration> config) : IRequestHandler<CreateMovementCommand, Guid>
{
    private readonly SectorConfiguration _config = config.Value;

    public async Task<Guid> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {
        // Business Rules Validation in Handler
        var fromSector = request.Request.FromSectorId.HasValue 
            ? await sectorRepository.GetByIdAsync(request.Request.FromSectorId.Value, cancellationToken)
            : null;
        
        var toSector = await sectorRepository.GetByIdAsync(request.Request.ToSectorId, cancellationToken);
        
        if (toSector == null)
            throw new ValidationException("Invalid target sector.");

        var hasExistingMovements = await movementRepository.HasMovementsAsync(request.Request.VolunteerId, cancellationToken);

        var validationError = Movement.BusinessRules.GetValidationError(
            fromSector?.Code,
            toSector.Code,
            hasExistingMovements,
            request.Request.IsGroupMovement,
            request.Request.GroupId,
            _config);

        if (!string.IsNullOrEmpty(validationError))
            throw new ValidationException(validationError);

        // Create Movement
        var movement = Movement.Create(
            volunteerId: request.Request.VolunteerId,
            fromSectorId: request.Request.FromSectorId,
            toSectorId: request.Request.ToSectorId,
            type: request.Request.Type,
            isGroupMovement: request.Request.IsGroupMovement,
            groupId: request.Request.GroupId,
            notes: request.Request.Notes);

        await movementRepository.AddAsync(movement, cancellationToken);
        return movement.Id;
    }
}

// Simplified Validator (no Repository dependencies)
public sealed class CreateMovementCommandValidator : AbstractValidator<CreateMovementCommand>
{
    public CreateMovementCommandValidator()
    {
        RuleFor(x => x.Request.VolunteerId)
            .NotEmpty()
            .WithMessage("Volunteer must be selected.");
        
        RuleFor(x => x.Request.ToSectorId)
            .NotEmpty()
            .WithMessage("Target sector must be selected.");
        
        RuleFor(x => x.Request.GroupId)
            .NotEmpty()
            .When(x => x.Request.IsGroupMovement)
            .WithMessage("Group ID is required for group movements.");
    }
}