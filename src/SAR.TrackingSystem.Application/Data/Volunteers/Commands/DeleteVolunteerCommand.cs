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
        var volunteer = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (volunteer == null)
            throw new ValidationException("Volunteer not found.");

        await repository.DeleteAsync(request.Id, cancellationToken);
        return true;
    }
}

public sealed class DeleteVolunteerCommandValidator : AbstractValidator<DeleteVolunteerCommand>
{
    public DeleteVolunteerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID cannot be empty.");
    }
}