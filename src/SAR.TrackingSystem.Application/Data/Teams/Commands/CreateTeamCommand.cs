using MediatR;
using SAR.TrackingSystem.Application.Repositories;
using SAR.TrackingSystem.Domain.Entities;
using FluentValidation;

namespace SAR.TrackingSystem.Application.Data.Teams.Commands;

public sealed record CreateTeamCommand(TeamRequest Request) : IRequest<Guid>;

public sealed class CreateTeamCommandHandler(
    ITeamRepository teamRepository) : IRequestHandler<CreateTeamCommand, Guid>
{
    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // Business Rules Validation
        var existingTeam = await teamRepository.GetByCodeAsync(request.Request.Code, cancellationToken);

        if (existingTeam != null)
        {
            throw new ValidationException("Bu kodla kayıtlı başka bir takım zaten var.");
        }

        // Create Team
        var team = new Team(
            code: request.Request.Code,
            name: request.Request.Name,
            capacity: request.Request.Capacity,
            isActive: request.Request.IsActive);

        await teamRepository.AddAsync(team, cancellationToken);
        return team.Id;
    }
}

public sealed class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Request.Code)
            .NotEmpty()
            .MaximumLength(10)
            .WithMessage("Takım kodu maksimum 10 karakter olmalıdır.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Takım adı maksimum 100 karakter olmalıdır.");

        RuleFor(x => x.Request.Capacity)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Kapasite 0 ile 100 arasında olmalıdır.");
    }
}