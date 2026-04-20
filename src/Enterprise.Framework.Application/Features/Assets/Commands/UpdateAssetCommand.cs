namespace Enterprise.Framework.Application.Features.Assets.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common.Enums;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateAssetCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public DateTime PurchaseDate { get; init; }
    public AssetStatus Status { get; init; }
    public long? AssignedEmployeeId { get; init; }
}

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Asset>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Asset), request.Id);
        }

        entity.Name = request.Name;
        entity.SerialNumber = request.SerialNumber;
        entity.PurchaseDate = request.PurchaseDate;
        entity.Status = request.Status;
        entity.AssignedEmployeeId = request.AssignedEmployeeId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
