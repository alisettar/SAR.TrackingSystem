using MediatR;
using SAR.TrackingSystem.Application.Data;
using SAR.TrackingSystem.Application.Repositories;

namespace SAR.TrackingSystem.Application.Data.Movements.Queries;

public sealed record GetMovementsQuery(PaginationRequest? PaginationRequest = null) : IRequest<PaginationResponse<MovementResponse>>;

public sealed class GetMovementsQueryHandler(IMovementRepository repository) 
    : IRequestHandler<GetMovementsQuery, PaginationResponse<MovementResponse>>
{
    public async Task<PaginationResponse<MovementResponse>> Handle(GetMovementsQuery request, CancellationToken cancellationToken)
    {
        var paginationRequest = request.PaginationRequest ?? new PaginationRequest();
        var (movements, totalCount) = await repository.GetPaginatedAsync(paginationRequest, cancellationToken);
        
        var responseList = MovementResponse.FromDomainList(movements);
        return new PaginationResponse<MovementResponse>(responseList, totalCount);
    }
}

public sealed record GetMovementByIdQuery(Guid Id) : IRequest<MovementResponse?>;

public sealed class GetMovementByIdQueryHandler(IMovementRepository repository)
    : IRequestHandler<GetMovementByIdQuery, MovementResponse?>
{
    public async Task<MovementResponse?> Handle(GetMovementByIdQuery request, CancellationToken cancellationToken)
    {
        var movement = await repository.GetByIdAsync(request.Id, cancellationToken);
        return movement == null ? null : MovementResponse.FromDomain(movement);
    }
}