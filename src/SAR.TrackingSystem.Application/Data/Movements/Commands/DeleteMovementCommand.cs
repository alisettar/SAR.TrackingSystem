using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using FluentValidation;

namespace SAR.TrackingSystem.Application.Data.Movements.Commands;

public sealed record DeleteMovementCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteMovementCommandHandler(IMovementRepository repository) 
    : IRequestHandler<DeleteMovementCommand, bool>
{
    public async Task<bool> Handle(DeleteMovementCommand request, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(request.Id, cancellationToken);
    }
}

public sealed class DeleteMovementCommandValidator : AbstractValidator<DeleteMovementCommand>
{
    public DeleteMovementCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Movement ID cannot be empty.");
    }
}
