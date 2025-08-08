using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using FluentValidation;

namespace SAR.TrackingSystem.Application.Data.Volunteers.Commands;

public sealed record DeleteVolunteerCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteVolunteerCommandHandler(IVolunteerRepository repository) 
    : IRequestHandler<DeleteVolunteerCommand, bool>
{
    public async Task<bool> Handle(DeleteVolunteerCommand request, CancellationToken cancellationToken)
    {
        await repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}

public sealed class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator(IVolunteerRepository repository)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID cannot be empty.")
            .MustAsync(async (id, cancellation) => 
                await repository.GetByIdAsync(id, cancellation) != null)
            .WithMessage("Volunteer not found.");
    }
}