namespace Enterprise.Framework.Application.Assets.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public record CreateAssetCommand : IRequest<long>
{
    public string Name { get; init; } = default!;
    public string SerialNumber { get; init; } = default!;
    public DateTime PurchaseDate { get; init; }
    public string Status { get; init; } = default!;
    public int? AssignedEmployeeId { get; init; }
}

public class CreateAssetCommandHandler : IRequestHandler<CreateAssetCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = new Asset
        {
            Name = request.Name,
            SerialNumber = request.SerialNumber,
            PurchaseDate = request.PurchaseDate,
            Status = request.Status,
            AssignedEmployeeId = request.AssignedEmployeeId
        };

        _context.Assets.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
