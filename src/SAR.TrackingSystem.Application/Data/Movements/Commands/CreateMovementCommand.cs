using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using FluentValidation;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record CreateMovementCommand(MovementRequest Request) : IRequest<Guid>;

public sealed class CreateMovementCommandHandler(IMovementRepository repository) 
    : IRequestHandler<CreateMovementCommand, Guid>
{
    public async Task<Guid> Handle(CreateMovementCommand request, CancellationToken cancellationToken)
    {
        var movement = Movement.Create(
            volunteerId: request.Request.VolunteerId,
            fromSectorId: request.Request.FromSectorId,
            toSectorId: request.Request.ToSectorId,
            type: request.Request.Type,
            isGroupMovement: request.Request.IsGroupMovement,
            groupId: request.Request.GroupId,
            notes: request.Request.Notes);

        await repository.AddAsync(movement, cancellationToken);
        return movement.Id;
    }
}

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